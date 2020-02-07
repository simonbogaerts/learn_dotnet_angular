import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { User } from 'src/app/models/user.model';
import { AlertService } from 'src/app/services/alert.service';
import { NgForm } from '@angular/forms';
import { UserService } from 'src/app/services/user.service';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  user: User;
  currentPhotoUrl: string;
  @ViewChild('editForm', null) editForm: NgForm;
  @HostListener('window:beforeunload', ['$event']) unloadNotification(
    $event: any
  ) {
    if (this.editForm.dirty) {
      $event.returnValue = true;
    }
  }

  constructor(
    private route: ActivatedRoute,
    private alertService: AlertService,
    private userService: UserService,
    private authService: AuthService
  ) {}

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.user = data['data'];
    });

    this.authService.currentPhotoUrl.subscribe(photoUrl => {
      this.currentPhotoUrl = photoUrl;
    });
  }

  updateUser() {
    const id = this.authService.decodedToken.nameid;
    console.log(id);
    this.userService.updateUser(id, this.user).subscribe(
      next => {
        this.alertService.success('Profile updated successfully!');

        this.editForm.reset(this.user);
      },
      error => {
        this.alertService.error(error);
      }
    );
  }

  updateMainPhoto(photoUrl: string) {
    this.user.photoUrl = photoUrl;
  }
}
