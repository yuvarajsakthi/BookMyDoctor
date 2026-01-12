import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { App } from './app/app';

bootstrapApplication(App, appConfig)
  .catch((err) => {
    // Application bootstrap failed - this is a critical error that should be logged
    // In production, you might want to send this to a logging service
    alert('Application failed to start. Please refresh the page.');
  });
