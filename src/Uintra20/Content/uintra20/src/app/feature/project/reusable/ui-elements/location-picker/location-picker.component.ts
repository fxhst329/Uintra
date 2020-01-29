
import { Component, ViewChild, OnInit, ElementRef, NgZone, Output, EventEmitter } from '@angular/core';
import { MapsAPILoader, MouseEvent } from '@agm/core';
import { GOOGLE_MAPS_CONFIG } from 'src/app/constants/maps/google-maps.const';
import { IGoogleMapsModel, ICoordinates } from './location-picker.interface';
import { GoogleGeolocationService } from './services/google-geolocation.service';

@Component({
  selector: 'app-location-picker',
  templateUrl: './location-picker.component.html',
  styleUrls: ['./location-picker.component.less']
})
export class LocationPickerComponent implements OnInit {

  address: string;
  @Output() addressChanged = new EventEmitter<string>();

  @ViewChild('search', { static: false })
  public searchElementRef: ElementRef;
  public googleMapsModel: IGoogleMapsModel;
  public defaultCoordinates: ICoordinates = GOOGLE_MAPS_CONFIG.DEFAULT_COORDINATES;

  constructor(
    private mapsAPILoader: MapsAPILoader,
    private ngZone: NgZone,
    private googleGeolocationService: GoogleGeolocationService
  ) { }

  public ngOnInit(): void {
    this.onInit();
    this.setupInputListener();
  }

  public handleMapClicked($event: MouseEvent): void {
    const latitude = $event.coords.lat;
    const longitude = $event.coords.lng;

    this.googleMapsModel.coordinates = {
      latitude,
      longitude,
    };
    this.updateDefaultCoordinates(latitude, longitude);
    this.googleGeolocationService.getAddress(latitude, longitude, r => {
      this.address = r;

      this.addressChanged.emit(this.address);
    });
  }

  private onInit(): void {
    this.address = '';
    this.googleMapsModel = {
      coordinates: GOOGLE_MAPS_CONFIG.DEFAULT_COORDINATES,
      zoom: GOOGLE_MAPS_CONFIG.ZOOM,
      disableDefaultUI: GOOGLE_MAPS_CONFIG.DISABLE_DEFAULT_UI,
      zoomControl: GOOGLE_MAPS_CONFIG.ZOOM_CONTROL
    };
  }

  private setupInputListener(): void {
    this.mapsAPILoader.load().then(() => {
      const autocomplete = new google.maps.places.Autocomplete(this.searchElementRef.nativeElement, {
        types: ["address"]
      });
      autocomplete.addListener("place_changed", () => {
        this.ngZone.run(() => {
          const place: google.maps.places.PlaceResult = autocomplete.getPlace();

          if (place.geometry === undefined || place.geometry === null) {
            return;
          }

          this.googleMapsModel.coordinates.latitude = place.geometry.location.lat();
          this.googleMapsModel.coordinates.longitude = place.geometry.location.lng();
          this.updateDefaultCoordinates(place.geometry.location.lat(), place.geometry.location.lng());
        });
      });
    });
  }

  private updateDefaultCoordinates(lat, long): void {
    this.defaultCoordinates.latitude = lat;
    this.defaultCoordinates.longitude = long;
  }
}
