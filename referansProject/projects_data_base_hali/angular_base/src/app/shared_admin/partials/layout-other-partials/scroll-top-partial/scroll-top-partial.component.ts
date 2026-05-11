import { Component, OnInit, AfterViewInit } from '@angular/core';
import KTLayoutScrolltop from 'src/assets/js/layout/extended/scrolltop';
import { KTUtil } from 'src/assets/js/components/util';


@Component({
  selector: 'app-scroll-top-partial',
  templateUrl: './scroll-top-partial.component.html',
  styleUrls: ['./scroll-top-partial.component.scss'],
})
export class ScrollTopPartialComponent implements OnInit, AfterViewInit {
  constructor() {}
  ngOnInit(): void {}
  ngAfterViewInit() {
    KTUtil.ready(() => {
      // Init Scrolltop
      KTLayoutScrolltop.init('kt_scrolltop');
    });
  }
}
