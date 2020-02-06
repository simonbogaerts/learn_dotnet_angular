import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Photo } from 'src/app/models/photo.model';
import { FileUploader } from 'ng2-file-upload';
import { environment } from 'src/environments/environment';
import { AuthService } from 'src/app/services/auth.service';
import { UserService } from 'src/app/services/user.service';
import { AlertService } from 'src/app/services/alert.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  @Input() photos: Photo[];
  @Output() GetMemberPhotoChange: EventEmitter<string> = new EventEmitter<
    string
  >();

  currentMain: Photo;
  uploader: FileUploader;
  hasBaseDropZoneOver: boolean;
  url = environment.apiUrl;

  constructor(
    private authService: AuthService,
    private userService: UserService,
    private alertService: AlertService
  ) {
    this.hasBaseDropZoneOver = false;
  }

  ngOnInit() {
    this.initializeUploader();
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url:
        this.url + 'users/' + this.authService.decodedToken.nameid + '/photos',
      authToken: 'Bearer ' + localStorage.getItem('token'),
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024
    });

    this.uploader.onAfterAddingFile = file => {
      file.withCredentials = false;
    };

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const res: Photo = JSON.parse(response);
        const photo = {
          id: res.id,
          url: res.url,
          dateAdded: res.dateAdded,
          description: res.description,
          isMain: res.isMain
        };
        this.photos.push(photo);
      }
    };
  }

  fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  setMainPhoto(photo: Photo) {
    this.userService
      .setMainPhoto(this.authService.decodedToken.nameid, photo.id)
      .subscribe(
        () => {
          this.currentMain = this.photos.filter(x => x.isMain === true)[0];
          this.currentMain.isMain = false;
          photo.isMain = true;
          this.authService.changeMemberPhoto(photo.url);
          this.authService.currentUser.photoUrl = photo.url;
          localStorage.setItem(
            'user',
            JSON.stringify(this.authService.currentUser)
          );
        },
        error => {
          this.alertService.error(error);
        }
      );
  }

  deletePhoto(id: number) {
    this.alertService.confirm(
      'Are you sure you want to delete this photo?',
      () => {
        this.userService
          .deletePhoto(this.authService.decodedToken.nameid, id)
          .subscribe(
            () => {
              this.photos.splice(
                this.photos.findIndex(x => x.id === id),
                1
              );
              this.alertService.success('Photo has been deleted!');
            },
            error => {
              this.alertService.error(
                'Something went wrong deleting the photo.'
              );
            }
          );
      }
    );
  }
}
