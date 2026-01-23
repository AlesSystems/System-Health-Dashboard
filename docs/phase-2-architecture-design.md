# PHASE 2 — Architecture Design

## 🧩 High-Level Architecture

```
+---------------------+
|       UI Layer      |
|  Charts, Controls   |
+----------▲----------+
           |
+----------|----------+
|   Application Core  |
|  State, Scheduler   |
+----------▲----------+
           |
+----------|----------+
|   Metrics Providers |
|  CPU / RAM / Disk   |
+---------------------+
```

## 🧠 Key Design Patterns

- Observer / Pub-Sub (metrics → UI)
- Strategy (OS-specific collectors)
- Dependency Injection
- Thread-safe data buffers
