# 🚀 BookTheShow React Web - Quick Start Guide

## 📦 Project Configuration

- **Framework**: React 19.1.0 + TypeScript 5.8.3
- **Build Tool**: Vite 6.3.5
- **Compiler**: Babel (via @vitejs/plugin-react)
- **Port**: 65044
- **Type**: TypeScript + JSX

---

## 🏃 Running Locally

### Option 1: Standard Development (Current Setup)

```powershell
# Navigate to Web project
cd c:\Workshops\Designs\BookTheShow\src\presentation\BookTheShow.Web

# Install dependencies (first time only)
npm install

# Start development server
npm run dev
```

**Access at**: http://localhost:65044

---

### Option 2: Upgrade to SWC (Faster Compilation - Recommended)

SWC is 20x faster than Babel for compilation.

```powershell
# Navigate to Web project
cd c:\Workshops\Designs\BookTheShow\src\presentation\BookTheShow.Web

# Remove Babel plugin
npm uninstall @vitejs/plugin-react

# Install SWC plugin
npm install -D @vitejs/plugin-react-swc

# Then update vite.config.ts (see below)
```

**Update `vite.config.ts`:**
```typescript
import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react-swc'; // Changed import

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [react()],
    server: {
        port: 65044,
        proxy: {
            // Proxy API requests to .NET backend
            '/api': {
                target: 'http://localhost:5000',
                changeOrigin: true,
                secure: false
            }
        }
    }
})
```

---

## 📜 Available Scripts

```powershell
# Development server with hot reload
npm run dev

# Type checking + production build
npm run build

# Preview production build
npm run preview

# Run ESLint
npm run lint
```

---

## 🔗 Connect to .NET API

To connect your React app to the BookTheShow.API:

1. **Update vite.config.ts** with proxy (see above)
2. **Start .NET API**:
   ```powershell
   cd c:\Workshops\Designs\BookTheShow\src\presentation\BookTheShow.API
   dotnet run
   ```
3. **Start React app**:
   ```powershell
   cd c:\Workshops\Designs\BookTheShow\src\presentation\BookTheShow.Web
   npm run dev
   ```

---

## 📦 Recommended Packages for BookTheShow

```powershell
# State Management
npm install @reduxjs/toolkit react-redux

# Routing
npm install react-router-dom

# HTTP Client
npm install axios

# Real-time (SignalR for seat updates)
npm install @microsoft/signalr

# UI Framework
npm install @mui/material @emotion/react @emotion/styled
# OR
npm install -D tailwindcss postcss autoprefixer
npx tailwindcss init -p

# Form Handling
npm install react-hook-form
npm install @hookform/resolvers yup

# Date Handling
npm install date-fns

# Icons
npm install @mui/icons-material
# OR
npm install lucide-react
```

---

## 📁 Recommended Folder Structure

```
src/
├── 📁 components/          # Reusable UI components
│   ├── common/
│   ├── layout/
│   └── features/
├── 📁 pages/               # Page components
│   ├── Home/
│   ├── Events/
│   ├── Booking/
│   └── Payment/
├── 📁 features/            # Feature-based modules
│   ├── events/
│   ├── booking/
│   └── user/
├── 📁 hooks/               # Custom React hooks
├── 📁 services/            # API service layers
│   ├── api.ts
│   ├── eventService.ts
│   └── bookingService.ts
├── 📁 store/               # Redux store
│   ├── index.ts
│   ├── slices/
│   └── middleware/
├── 📁 types/               # TypeScript types
├── 📁 utils/               # Utility functions
├── 📁 assets/              # Static assets
└── 📁 styles/              # Global styles
```

---

## 🎨 Next Steps

1. ✅ Verify React app runs: `npm run dev`
2. ⬆️ Switch to SWC for faster builds
3. 📦 Install additional packages (Redux, Router, etc.)
4. 🔌 Set up API proxy to connect with .NET backend
5. 🎨 Choose UI framework (MUI or Tailwind)
6. 🏗️ Create folder structure for features

---

## 🐛 Troubleshooting

### Port Already in Use
```powershell
# Change port in vite.config.ts
server: {
    port: 3000, // Change to any available port
}
```

### Node Modules Issues
```powershell
# Clear and reinstall
Remove-Item -Recurse -Force node_modules
Remove-Item package-lock.json
npm install
```

### TypeScript Errors
```powershell
# Clear TypeScript cache
Remove-Item -Recurse -Force node_modules/.tmp
npm run dev
```

---

**Current Status**: ✅ TypeScript + React 19 + Vite configured
**Recommended Upgrade**: Switch to SWC for 20x faster compilation
**Ready to run**: `npm run dev` → http://localhost:65044
