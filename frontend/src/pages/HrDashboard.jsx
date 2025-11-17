import React, { useEffect, useState } from 'react';
import { getCandidatesByJobId } from '../api/candidateService';
import CandidateList from '../components/CandidateList';

const HrDashboard = () => {
    const [jobId, setJobId] = useState(null);
    const [candidates, setCandidates] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        // Fetch candidates for a specific job posting
        const fetchCandidates = async () => {
            if (jobId) {
                const fetchedCandidates = await getCandidatesByJobId(jobId);
                setCandidates(fetchedCandidates);
                setLoading(false);
            }
        };

        fetchCandidates();
    }, [jobId]);

    const handleJobSelection = (selectedJobId) => {
        setJobId(selectedJobId);
        setLoading(true);
    };

    return (
        <div>
            <h1>HR Dashboard</h1>
            <div>
                <label htmlFor="jobSelect">Select Job Posting:</label>
                <select id="jobSelect" onChange={(e) => handleJobSelection(e.target.value)}>
                    {/* Options for job postings should be populated here */}
                </select>
            </div>
            {loading ? (
                <p>Loading candidates...</p>
            ) : (
                <CandidateList candidates={candidates} />
            )}
        </div>
    );
};

export default HrDashboard;