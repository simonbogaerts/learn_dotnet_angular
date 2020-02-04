import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { User } from '../models/user.model';
import { UserService } from '../services/user.service';
import { AlertService } from '../services/alert.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../services/auth.service';

@Injectable()
export class MemberEditResolver implements Resolve<User> {
  constructor(
    private userService: UserService,
    private alertService: AlertService,
    private authService: AuthService,
    private router: Router
  ) {}

  resolve(route: ActivatedRouteSnapshot): Observable<User> {
    const id = this.authService.decodedToken.nameid;

    return this.userService.getUser(id).pipe(
      catchError(error => {
        this.alertService.error('Problem retrieving data');
        this.router.navigate(['/members']);

        return of(null);
      })
    );
  }
}
