import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private auth: AuthService,private router: Router, private toastr: ToastrService) { }
  canActivate(): boolean{
    if(this.auth.isLoggedIn()){
      return true;
    }
    else{
      this.router.navigate(['login']);
      this.toastr.error("You are not logged in!", 'ERROR', { timeOut: 2000, positionClass: 'toast-top-right'});
      return false;
    }
  }
  
}
