import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { NgToastService } from 'ng-angular-popup';
import ValidateForm from 'src/app/helpers/validateform';
import { AuthService } from 'src/app/services/auth.service';
import { ConfirmedValidator } from 'src/app/helpers/validateform';

@Component({
  selector: 'app-signup',
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.css']
})
export class SignupComponent implements OnInit {

  signUpForm!: FormGroup;
  constructor(private fb:FormBuilder, private auth: AuthService, private router: Router, private toast: NgToastService) { }

  ngOnInit(): void {
    this.signUpForm = this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      username: ['', Validators.required],
      //gender: ['', Validators.required],
      email: ['', Validators.required],
      password: ['', Validators.required],
      confirmPassword: ['', Validators.required]
    }, { 
      validator: ConfirmedValidator('password', 'confirmPassword')
    });
  }
  password(formGroup: FormGroup) {
    var password = formGroup.get('password');
    var confirmPassword = formGroup.get('confirmpassword');
    return password === confirmPassword ? null : { passwordNotMatch: true };
  }

  onSignUp(){
    if(this.signUpForm.valid){
      console.log(this.signUpForm.value);
      this.auth.signUp(this.signUpForm.value).subscribe({
        next: (res => {
          //alert(res.message);
          this.toast.success({detail:"SUCCESS",summary:res.message, duration:3400});
          this.signUpForm.reset();
          this.router.navigate(['login']);
        }),
        error: (err => {
          this.toast.warning({detail:"WARNING",summary:err.error.message, duration:3400});
        })
      })
    }
    else{
      //console.log("Invalid SignUpForm");
      ValidateForm.validateAllFormFields(this.signUpForm);
      this.toast.error({detail:"ERROR",summary:"Invalid Signup Form", duration:3400});
    }
  }

}



