import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router, RouterFeature } from '@angular/router';
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
  constructor(private fb: FormBuilder, private auth: AuthService, private router: Router) { }

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
          //alert(res.message);
          this.loginForm.reset();
          this.router.navigate(['dashboard']);
        }),
        error: (err => {
          alert(err?.error.message);
        })
      })
    }
    else{
      console.log("Invalid LoginForm");
      ValidateForm.validateAllFormFields(this.loginForm);
      alert("Invalid LoginForm")
    }
  }

  Empty(){
    this.submitted = true;
    if(this.loginForm.invalid) return;
  }
}


