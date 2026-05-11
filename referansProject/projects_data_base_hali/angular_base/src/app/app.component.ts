// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\app.component.ts
import { Component, ChangeDetectionStrategy, OnDestroy, OnInit } from '@angular/core';
import { Router, NavigationEnd, NavigationError } from '@angular/router';
import { Subscription } from 'rxjs';
import { SplashScreenService } from './shared_admin/template/splash-screen/splash-screen.service';

@Component({
    selector: 'body[root]',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss'],
    changeDetection: ChangeDetectionStrategy.Default,
})
export class AppComponent implements OnInit, OnDestroy {
    private unsubscribe: Subscription[] = [];

    constructor(
        private splashScreenService: SplashScreenService,
        private router: Router
    ) { }

    ngOnInit() {
        const routerSubscription = this.router.events.subscribe((event) => {
            if (event instanceof NavigationEnd) {
                this.splashScreenService.hide();
                window.scrollTo(0, 0);
                setTimeout(() => {
                    document.body.classList.add('page-loaded');
                }, 500);
            }
        });
        this.unsubscribe.push(routerSubscription);
    }

    ngOnDestroy() {
        this.unsubscribe.forEach((sb) => sb.unsubscribe());
    }
}
