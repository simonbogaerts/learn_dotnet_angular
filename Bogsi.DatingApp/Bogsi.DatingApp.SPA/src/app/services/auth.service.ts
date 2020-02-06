import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { JwtHelperService } from '@auth0/angular-jwt';
import { environment } from 'src/environments/environment';
import { User } from '../models/user.model';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  baseUrl: string = environment.apiUrl + 'auth/';
  jwtHelper: JwtHelperService = new JwtHelperService();
  decodedToken: any;
  currentUser: User;
  photoUrl: BehaviorSubject<string> = new BehaviorSubject<string>(
    '../../assets/user.png'
  );
  currentPhotoUrl = this.photoUrl.asObservable();

  constructor(private http: HttpClient) {}

  login(model: any) {
    return this.http.post(this.baseUrl + 'login', model).pipe(
      map((response: any) => {
        const user = response;

        if (user) {
          localStorage.setItem('token', user.token);
          localStorage.setItem('user', JSON.stringify(user.user));

          this.setDecodedToken(user.token);
          this.setCurrentUser(user.user);
          this.changeMemberPhoto(this.currentUser.photoUrl);
        }
      })
    );
  }

  register(model: any) {
    return this.http.post(this.baseUrl + 'register', model);
  }

  isLoggedIn() {
    const token = localStorage.getItem('token');

    return !this.jwtHelper.isTokenExpired(token);
  }

  setDecodedToken(token: any) {
    this.decodedToken = this.jwtHelper.decodeToken(token);
  }

  setCurrentUser(user: User) {
    this.currentUser = user;
  }

  changeMemberPhoto(photoUrl: string) {
    this.photoUrl.next(photoUrl);
  }
}
