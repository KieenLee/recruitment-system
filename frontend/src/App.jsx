import React from "react";
import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import LandingPage from "./pages/LandingPage";
import JobDetailPage from "./pages/JobDetailPage";
import LoginPage from "./pages/LoginPage";
import Dashboard from "./components/Dashboard";
import JobList from "./components/JobList";
import CandidateList from "./components/CandidateList";
import ProtectedRoute from "./components/ProtectedRoute";
import HRLayout from "./components/HRLayout";
import "./App.css";

function App() {
  return (
    <BrowserRouter>
      <Routes>
        {/* Public Routes - For Candidates */}
        <Route path="/" element={<LandingPage />} />
        <Route path="/jobs/:jobId" element={<JobDetailPage />} />
        <Route path="/login" element={<LoginPage />} />

        {/* Protected Routes - For HR */}
        <Route
          path="/hr/*"
          element={
            <ProtectedRoute>
              <HRLayout />
            </ProtectedRoute>
          }
        >
          <Route path="dashboard" element={<Dashboard />} />
          <Route path="jobs" element={<JobList />} />
          <Route path="jobs/:jobId/candidates" element={<CandidateList />} />
          <Route index element={<Navigate to="/hr/dashboard" replace />} />
        </Route>

        {/* Fallback */}
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
