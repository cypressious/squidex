/*
 * Squidex Headless CMS
 *
 * @license
 * Copyright (c) Sebastian Stehle. All rights reserved
 */

import { Component } from '@angular/core';
import { Router } from '@angular/router';

import { AuthService } from 'shared';

@Component({
    selector: 'home-page',
    styleUrls: ['./home-page.component.scss'],
    templateUrl: './home-page.component.html'
})
export class HomePageComponent {
    public showLoginError = false;

    constructor(
        private readonly auth: AuthService,
        private readonly router: Router
    ) {
    }

    public login() {
        if (this.isIE()) {
            this.auth.loginRedirect();
        } else {
            this.auth.loginPopup()
                .subscribe(() => {
                    this.router.navigate(['/app']);
                }, ex => {
                    this.showLoginError = true;
                });
        }
    }

    public isIE() {
        const isIE = !!navigator.userAgent.match(/Trident/g) || !!navigator.userAgent.match(/MSIE/g);

        return isIE;
    }
}