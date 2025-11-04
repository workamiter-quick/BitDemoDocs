import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HashLocationStrategy, LocationStrategy } from '@angular/common';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { MapViewComponent } from './map-view/map-view.component';
import { AdminViewComponent } from './admin-view/admin-view.component';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { SearchAttributeComponent } from './map-view/search-attribute/search-attribute.component';
import { FormsModule } from '@angular/forms';
import { UserBookmarkComponent } from './map-view/user-bookmark/user-bookmark.component';
import { FeatureCaptureComponent } from './map-view/feature-capture/feature-capture.component';
import { LayerManangerComponent } from './layer-mananger/layer-mananger.component';


@NgModule({
  declarations: [
    AppComponent,
    MapViewComponent,
    AdminViewComponent,
    PageNotFoundComponent,
    SearchAttributeComponent,
    UserBookmarkComponent,
    FeatureCaptureComponent,
    LayerManangerComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    RouterModule.forRoot([
      { path: 'mapview', component: MapViewComponent },
      { path: 'admin', component: AdminViewComponent },
      { path: 'layermanager', component: LayerManangerComponent },
      { path: '', redirectTo: '/mapview', pathMatch: 'full' },
      { path: 'a', redirectTo: '/admin', pathMatch: 'full' },
      { path: 'm', redirectTo: '/mapview', pathMatch: 'full' },
      { path: 'l', redirectTo: '/layermananger', pathMatch: 'full' },
      { path: '**', component: PageNotFoundComponent },
    ]),
    NgbModule,
    HttpClientModule,
    FormsModule
  ],
  providers: [{provide: LocationStrategy, useClass: HashLocationStrategy}],
  bootstrap: [AppComponent]
})
export class AppModule { }
