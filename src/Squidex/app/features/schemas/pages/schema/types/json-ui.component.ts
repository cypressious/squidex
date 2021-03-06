/*
 * Squidex Headless CMS
 *
 * @license
 * Copyright (c) Sebastian Stehle. All rights reserved
 */

import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { FormGroup } from '@angular/forms';

import { JsonFieldPropertiesDto } from 'shared';

@Component({
    selector: 'sqx-json-ui',
    styleUrls: ['json-ui.component.scss'],
    templateUrl: 'json-ui.component.html',
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class JsonUIComponent {
    @Input()
    public editForm: FormGroup;

    @Input()
    public properties: JsonFieldPropertiesDto;
}