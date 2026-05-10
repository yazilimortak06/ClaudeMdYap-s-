// Kaynak: E:\Projeler\Angular\PixdinnNewMenu\src\app\pages\home\home.component.ts
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  public closeBanner : boolean | undefined;
  constructor() { }
  toggleBanner(){
    this.closeBanner = !this.closeBanner;
  }

  ngOnInit(): void {
  }

}
