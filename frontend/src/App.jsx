import {
  BrowserRouter as Router,
  Routes,
  Route,
  Navigate,
} from "react-router-dom";
import Dashboard from "./components/Dashboard";
import HrDashboard from "./pages/HrDashboard";
import CandidatePortal from "./pages/CandidatePortal";
import "./css/App.css";

function App() {
  return (
    <Router>
      <div className="app">
        <Routes>
          {/* Default route - redirect to jobs dashboard */}
          <Route path="/" element={<Navigate to="/jobs" replace />} />

          {/* HR Dashboard - List all jobs */}
          <Route path="/jobs" element={<Dashboard />} />

          {/* HR Dashboard - View specific job and candidates */}
          <Route path="/jobs/:jobId/dashboard" element={<HrDashboard />} />

          {/* Candidate Portal - Apply to job */}
          <Route path="/apply/:jobId" element={<CandidatePortal />} />

          {/* 404 Not Found */}
          <Route
            path="*"
            element={
              <div className="not-found">
                <h1>404</h1>
                <p>Page Not Found</p>
                <a href="/jobs">Back to Dashboard</a>
              </div>
            }
          />
        </Routes>
      </div>
    </Router>
  );
}

export default App;
