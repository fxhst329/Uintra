import { Component, OnInit, Input, Output, EventEmitter } from "@angular/core";
import { IDatePickerOptions } from "src/app/feature/shared/interfaces/DatePickerOptions";
import * as moment from "moment";
import { IDatepickerData } from "../../../reusable/inputs/datepicker-from-to/datepiker-from-to.interface";
import { PinActivityService } from "./pin-activity.service";

export interface IPinedData {
  isPinCheked: boolean;
  isAccepted: boolean;
  pinDate: string;
}
@Component({
  selector: "app-pin-activity",
  templateUrl: "./pin-activity.component.html",
  styleUrls: ["./pin-activity.component.less"]
})
export class PinActivityComponent implements OnInit {
  @Input() isPinCheked: boolean;
  @Input() isAccepted: boolean;
  @Input() endPinDate: string;
  @Output() handleChange = new EventEmitter<IPinedData>();

  @Input() publishDate: string = null;
  @Input() unpublishDate: string = null;

  options: IDatePickerOptions;
  pinDate = null;
  pinedDateValue: IPinedData = {
    isPinCheked: false,
    isAccepted: false,
    pinDate: ""
  };

  constructor(private pinActivityService: PinActivityService) {
    this.pinActivityService.publishDates$.subscribe((dates: IDatepickerData) => {
      this.options = {
        ...this.options,
        minDate: dates.from ? moment(dates.from).subtract(5, "minutes") : false,
        maxDate: dates.to ? moment(dates.to).add(5, "minutes") : false
      };
    });
  }

  ngOnInit() {
    this.pinedDateValue = {
      isPinCheked: this.isPinCheked,
      isAccepted: this.isAccepted,
      pinDate: this.endPinDate
    };

    this.options = {
      showClose: true,
    };

    this.pinDate = this.endPinDate ? moment(this.endPinDate) : moment();
  }

  onDateChange() {
    this.pinedDateValue.pinDate = this.pinDate ? this.pinDate.format() : "";
    this.handleChange.emit(this.pinedDateValue);
  }
  onAcceptedChange() {
    this.pinedDateValue.isPinCheked = this.isPinCheked;
    this.handleChange.emit(this.pinedDateValue);
  }
}
