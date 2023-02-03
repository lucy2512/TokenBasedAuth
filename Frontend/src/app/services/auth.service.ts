import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private baseUrl:string = "https://localhost:7247/api/User/";
  constructor(private http: HttpClient,private router: Router) { }

  signUp(userObj:any){
    return this.http.post<any>(`${this.baseUrl}Register`, userObj);
  }

  login(loginObj:any){
    return this.http.post<any>(`${this.baseUrl}Login`, loginObj);
  }

  signOut(){
    localStorage.removeItem('token');
    this.router.navigate(['login']);
  }

  storeToken(tokenValue:string){
    localStorage.setItem('token', tokenValue);
  }

  getToken(){
    return localStorage.getItem('token');
  }

  isLoggedIn(){
    return !!localStorage.getItem('token');
  }
}
