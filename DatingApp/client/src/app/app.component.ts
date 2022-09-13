import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title: string = 'The Dating App';
  users: any;

  constructor(private http:HttpClient) {
  
  }

  ngOnInit(): void {
    this.getUsers();
  }

  //rxjs is a library that allows us to asynchronous operation 
  getUsers(){
    this.http.get('https://localhost:5001/api/users').subscribe(
      // response => {
      //   this.users = response;
      // },
      // error => {
      //   console.log(error);
      // },
      // () => {
      //   console.log('Finished');
      // }
      {
        next: response => {this.users = response;}, //what to do with returned data
        error: error => {console.log(error);},      //what to do with error
        complete: () => {console.log('Finished');}    // what to do when finished
      }
    )
  }

}
