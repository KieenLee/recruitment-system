import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import CandidatePortal from './pages/CandidatePortal';
import HrDashboard from './pages/HrDashboard';
import Dashboard from './components/Dashboard';

const App = () => {
    return (
        <Router>
            <Routes>
                <Route path="/" element={<Dashboard />} />
                <Route path="/candidate-portal" element={<CandidatePortal />} />
                <Route path="/hr-dashboard" element={<HrDashboard />} />
            </Routes>
        </Router>
    );
};

export default App;