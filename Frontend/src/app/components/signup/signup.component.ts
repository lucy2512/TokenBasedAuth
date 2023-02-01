import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import ValidateForm from 'src/app/helpers/validateform';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-signup',
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.css']
})
export class SignupComponent implements OnInit {

  signUpForm!: FormGroup;
  constructor(private fb:FormBuilder, private auth: AuthService, private router: Router) { }

  ngOnInit(): void {
    this.signUpForm = this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      username: ['', Validators.required],
      //gender: ['', Validators.required],
      email: ['', Validators.required],
      password: ['', Validators.required],
      confirmPassword: ['', Validators.required]
    });
  }

  onSignUp(){
    if(this.signUpForm.valid){
      console.log(this.signUpForm.value);
      this.auth.signUp(this.signUpForm.value).subscribe({
        next: (res => {
          alert(res.message);
          this.signUpForm.reset();
          this.router.navigate(['login']);
        }),
        error: (err => {
          alert(err?.error.message);
        })
      })
    }
    else{
      console.log("Invalid SignUpForm");
      ValidateForm.validateAllFormFields(this.signUpForm);
      alert("Invalid SignUpForm")
    }
  }

}



