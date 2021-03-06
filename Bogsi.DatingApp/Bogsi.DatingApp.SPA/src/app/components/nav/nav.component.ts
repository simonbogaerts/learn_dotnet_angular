import { Component, OnInit } from '@angular/core';
import { AlertService } from 'src/app/services/alert.service';
import { AuthService } from 'src/app/services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};
  photoUrl: string;

  constructor(
    public authService: AuthService,
    private alertService: AlertService,
    private router: Router
  ) {}

  ngOnInit() {
    this.authService.currentPhotoUrl.subscribe(photoUrl => {
      this.photoUrl = photoUrl;
    });
  }

  login() {
    this.authService.login(this.model).subscribe(
      next => {
        this.alertService.success('Logged in successfully!');
      },
      error => {
        this.alertService.error('Failed to login!');
      },
      () => {
        this.router.navigate(['/members']);
      }
    );
  }

  loggedIn() {
    return this.authService.isLoggedIn();
  }

  logOut() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');

    this.authService.decodedToken = null;
    this.authService.currentUser = null;

    this.router.navigate(['/home']);
    this.alertService.message('Logged out successfully!');
  }
}
