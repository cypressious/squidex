/*
 * Squidex Headless CMS
 *
 * @license
 * Copyright (c) Sebastian Stehle. All rights reserved
 */

import { ChangeDetectionStrategy, Component, Input, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { Observable } from 'rxjs';

import { NumberFieldPropertiesDto, ValidatorsEx } from 'shared';

@Component({
    selector: 'sqx-date-time-validation',
    styleUrls: ['date-time-validation.component.scss'],
    templateUrl: 'date-time-validation.component.html',
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class DateTimeValidationComponent implements OnInit {
    @Input()
    public editForm: FormGroup;

    @Input()
    public properties: NumberFieldPropertiesDto;

    public hideDefaultValue: Observable<boolean>;

    public ngOnInit() {
        this.editForm.addControl('maxValue',
            new FormControl(this.properties.maxValue, [
                ValidatorsEx.validDateTime()
            ]));

        this.editForm.addControl('minValue',
            new FormControl(this.properties.minValue, [
                ValidatorsEx.validDateTime()
            ]));

        this.editForm.addControl('defaultValue',
            new FormControl(this.properties.defaultValue, [
                ValidatorsEx.validDateTime()
            ]));

        this.hideDefaultValue =
            this.editForm.get('isRequired').valueChanges
                .startWith(this.properties.isRequired)
                .map(x => !!x);
    }
}