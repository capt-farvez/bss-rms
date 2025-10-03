# BSS - Restaurant Management System - Single Page Application

## Project Setup Instructions

### Prerequisites

Before you begin, ensure you have the following installed:
- **Node.js** (v18.x or higher) - [Download Node.js](https://nodejs.org/)
- **npm** (v9.x or higher) - Comes with Node.js
- **Angular CLI** (v18.x) - Install globally via npm

### Installation Steps

1. **Clone the Repository**
   ```bash
   git clone https://github.com/capt-farvez/bss-rms.git
   cd bss-rms/bss-rms-spa
   ```

2. **Install Angular CLI (if not already installed)**
   ```bash
   npm install -g @angular/cli@18
   ```

3. **Install Project Dependencies**
   ```bash
   npm install
   ```

### Build Instructions

1. **Development Build**
   ```bash
   npm run build
   ```

2. **Production Build**
   ```bash
   npm run build:prod
   ```
   or
   ```bash
   ng build --configuration production
   ```

3. **Build Output**
   - The build artifacts will be stored in the `dist/` directory
   - Production builds include optimizations like minification and tree-shaking

### Development Server

1. **Start the Development Server**
   ```bash
   npm start
   ```
   or
   ```bash
   ng serve
   ```

2. **Access the Application**
   - Open your browser and navigate to `http://localhost:4200/`
   - The application will automatically reload when you make changes to the source files