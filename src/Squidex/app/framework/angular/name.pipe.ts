/*
 * Squidex Headless CMS
 *
 * @license
 * Copyright (c) Sebastian Stehle. All rights reserved
 */

import { Pipe } from '@angular/core';

import { StringHelper } from './../utils/string-helper';

@Pipe({
    name: 'displayName'
})
export class DisplayNamePipe {
    public transform(value: any, field1 = 'label', field2  = 'name'): any {
        if (!value) {
            return '';
        }

        return StringHelper.firstNonEmpty(this.valueOf(value, field1), this.valueOf(value, field2));
    }

    private valueOf(o: any, s: string): any {
        s = s.replace(/\[(\w+)\]/g, '.$1');
        s = s.replace(/^\./, '');

        const parts = s.split('.');

        for (let i = 0, n = parts.length; i < n; ++i) {
            let k = parts[i];

            if (k in o) {
                o = o[k];
            } else {
                return;
            }
        }

        return o;
    }
}