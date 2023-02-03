import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router, RouterFeature } from '@angular/router';
import { NgToastService } from 'ng-angular-popup';
import { ToastrService } from 'ngx-toastr';
import ValidateForm from 'src/app/helpers/validateform';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  loginForm!: FormGroup;
  submitted = false;
  constructor(private fb: FormBuilder, private auth: AuthService, private router: Router, private toast: NgToastService,private toastr:ToastrService) { }

  ngOnInit(): void {
    this.loginForm = this.fb.group({
      username: ['',Validators.required],
      password: ['',Validators.required]
    });
  }

  onLogin(){ 
    if(this.loginForm.valid){
      console.log(this.loginForm.value);
      this.auth.login(this.loginForm.value).subscribe({
        next: (res => {
          this.auth.storeToken(res.token);
          //alert(res.message);
          //this.toast.success({detail:"SUCCESS",summary:res.message, duration:2000});
          this.toastr.success(res.message, 'SUCCESS', { timeOut: 2000, positionClass: 'toast-top-right' });
          this.loginForm.reset();
          this.router.navigate(['dashboard']);
        }),
        error: (err => {
          this.toastr.error(err.error.message, 'ERROR', { timeOut: 2000 });
        })
      })
    }
    else{
      console.log("Invalid LoginForm");
      ValidateForm.validateAllFormFields(this.loginForm);
      this.toastr.error("Invalid login form!", 'ERROR', { timeOut: 2000 });
    }
  }

  Empty(){
    this.submitted = true;
    if(this.loginForm.invalid) return;
  }
}


