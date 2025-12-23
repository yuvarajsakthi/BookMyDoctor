import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Header } from './layout/header/header';
import { NgxSnowfallComponent } from 'ngx-snowfall';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Header, NgxSnowfallComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
}