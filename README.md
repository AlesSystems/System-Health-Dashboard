# System Health Dashboard 📊

A real-time desktop application for comprehensive system monitoring and performance tracking.

## 🎯 Project Goals

Build a powerful, cross-platform desktop application that provides:

- **CPU Usage Monitoring** - Real-time CPU utilization, per-core metrics, and process tracking
- **Memory Usage Tracking** - RAM usage, swap memory, and memory-intensive process identification
- **Disk Activity Monitoring** - Read/write speeds, disk usage, and I/O operations
- **Network Activity** - Upload/download speeds, network interfaces, and bandwidth usage
- **System Alerts** - Configurable thresholds and notifications for critical metrics
- **Historical Trends** - Time-series data visualization and performance analytics

## 🛠️ Tech Stack

### Frontend Framework
- **Electron** - Cross-platform desktop application framework
- **React** - UI component library for building interactive interfaces
- **TypeScript** - Type-safe development

### Visualization
- **Chart.js** or **Recharts** - For real-time graphs and historical data visualization
- **D3.js** (optional) - For advanced custom visualizations

### System Monitoring
- **systeminformation** - Cross-platform system and hardware information library
- **node-os-utils** - CPU, memory, and disk utilities
- **speedtest-net** (optional) - Network speed testing

### State Management
- **Redux Toolkit** or **Zustand** - For managing application state
- **React Query** - For data fetching and caching

### Styling
- **Tailwind CSS** or **Material-UI** - For modern, responsive design
- **CSS Modules** or **Styled Components** - Component-level styling

### Build Tools
- **Vite** or **Webpack** - Module bundler and development server
- **Electron Builder** - Package and distribute the application

### Testing
- **Jest** - Unit testing framework
- **React Testing Library** - Component testing
- **Playwright** - E2E testing for Electron apps

## 📁 Proposed File Structure

```
System-Health-Dashboard/
├── .github/                    # GitHub workflows and templates
│   └── workflows/
│       ├── build.yml          # Build and test automation
│       └── release.yml        # Release automation
├── public/                    # Static assets
│   ├── icons/                # Application icons
│   └── index.html            # Main HTML file
├── src/
│   ├── main/                 # Electron main process
│   │   ├── main.ts           # Main entry point
│   │   ├── preload.ts        # Preload script
│   │   └── ipc/              # IPC handlers
│   │       ├── cpu.ts
│   │       ├── memory.ts
│   │       ├── disk.ts
│   │       └── network.ts
│   ├── renderer/             # React renderer process
│   │   ├── App.tsx           # Main React component
│   │   ├── index.tsx         # Renderer entry point
│   │   ├── components/       # React components
│   │   │   ├── Dashboard/
│   │   │   │   └── Dashboard.tsx
│   │   │   ├── CPUMonitor/
│   │   │   │   └── CPUMonitor.tsx
│   │   │   ├── MemoryMonitor/
│   │   │   │   └── MemoryMonitor.tsx
│   │   │   ├── DiskMonitor/
│   │   │   │   └── DiskMonitor.tsx
│   │   │   ├── NetworkMonitor/
│   │   │   │   └── NetworkMonitor.tsx
│   │   │   ├── AlertsPanel/
│   │   │   │   └── AlertsPanel.tsx
│   │   │   └── HistoricalCharts/
│   │   │       └── HistoricalCharts.tsx
│   │   ├── hooks/            # Custom React hooks
│   │   │   ├── useSystemInfo.ts
│   │   │   └── useAlerts.ts
│   │   ├── store/            # State management
│   │   │   ├── index.ts
│   │   │   └── slices/
│   │   │       ├── systemSlice.ts
│   │   │       └── alertsSlice.ts
│   │   ├── utils/            # Utility functions
│   │   │   ├── formatters.ts
│   │   │   └── calculations.ts
│   │   └── styles/           # Global styles
│   │       └── global.css
│   └── shared/               # Shared types and constants
│       ├── types.ts
│       └── constants.ts
├── tests/                    # Test files
│   ├── unit/
│   ├── integration/
│   └── e2e/
├── docs/                     # Documentation
│   ├── API.md
│   ├── ARCHITECTURE.md
│   └── CONTRIBUTING.md
├── dist/                     # Built application (gitignored)
├── node_modules/             # Dependencies (gitignored)
├── .env.example              # Environment variables template
├── .eslintrc.json           # ESLint configuration
├── .gitignore               # Git ignore rules
├── .prettierrc              # Prettier configuration
├── electron-builder.json    # Electron builder configuration
├── package.json             # Project dependencies and scripts
├── tsconfig.json            # TypeScript configuration
├── vite.config.ts           # Vite configuration
├── LICENSE                  # MIT License
└── README.md               # This file
```

## 🚀 Getting Started

### Prerequisites

- **Node.js** (v18 or higher)
- **npm** or **yarn** package manager
- **Git**

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/AlesSystems/System-Health-Dashboard.git
   cd System-Health-Dashboard
   ```

2. Install dependencies:
   ```bash
   npm install
   # or
   yarn install
   ```

3. Set up environment variables:
   ```bash
   cp .env.example .env
   ```

### Development

1. Start the development server:
   ```bash
   npm run dev
   # or
   yarn dev
   ```

2. The application will launch in development mode with hot reload enabled.

### Building

Build the application for production:

```bash
# Build for current platform
npm run build

# Build for specific platforms
npm run build:win    # Windows
npm run build:mac    # macOS
npm run build:linux  # Linux
```

### Testing

```bash
# Run unit tests
npm run test

# Run tests in watch mode
npm run test:watch

# Run E2E tests
npm run test:e2e

# Generate coverage report
npm run test:coverage
```

## 📊 Features

### Real-time Monitoring
- Live updates of system metrics (1-second intervals)
- Visual indicators for resource utilization
- Color-coded alerts for threshold breaches

### Historical Analytics
- Time-series charts for all metrics
- Customizable time ranges (1h, 6h, 24h, 7d, 30d)
- Export data to CSV/JSON

### Alerts & Notifications
- Configurable threshold alerts
- Desktop notifications
- Alert history and logging

### System Information
- Detailed hardware specifications
- OS information and version
- Running processes overview

### Customization
- Light/Dark theme support
- Customizable refresh intervals
- Widget layout customization

## 🎨 UI/UX Design Principles

- **Clean and Minimal** - Focus on data, not clutter
- **Responsive** - Adapts to different window sizes
- **Accessible** - Keyboard navigation and screen reader support
- **Performance** - Optimized rendering for real-time updates

## 🔧 Configuration

The application can be configured through:

- **Settings UI** - User preferences and thresholds
- **Config File** - `~/.system-health-dashboard/config.json`
- **Environment Variables** - For advanced configuration

## 🤝 Contributing

We welcome contributions! Please see [CONTRIBUTING.md](docs/CONTRIBUTING.md) for guidelines.

### Development Workflow

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Code Standards

- Follow the existing code style
- Write meaningful commit messages
- Add tests for new features
- Update documentation as needed

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- **Electron** - For the cross-platform desktop framework
- **systeminformation** - For comprehensive system metrics
- **React** - For the powerful UI framework
- Community contributors and supporters

## 📮 Support

- **Issues** - Report bugs or request features via [GitHub Issues](https://github.com/AlesSystems/System-Health-Dashboard/issues)
- **Discussions** - Join conversations in [GitHub Discussions](https://github.com/AlesSystems/System-Health-Dashboard/discussions)

## 🗺️ Roadmap

- [x] Project initialization and documentation
- [ ] Core monitoring engine
- [ ] Basic UI implementation
- [ ] Real-time data visualization
- [ ] Alert system
- [ ] Historical data storage
- [ ] Multi-platform builds
- [ ] Auto-update functionality
- [ ] Plugin system for extensibility
- [ ] Cloud sync capabilities (optional)

---

Built with ❤️ by AlesSystems
