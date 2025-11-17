import React, { useEffect, useState } from 'react';
import { getCandidatesByJobId } from '../api/candidateService';
import CandidateList from '../components/CandidateList';

const CandidatePortal = () => {
    const [candidates, setCandidates] = useState([]);
    const [jobId, setJobId] = useState(null); // This should be set based on the selected job

    useEffect(() => {
        if (jobId) {
            const fetchCandidates = async () => {
                const data = await getCandidatesByJobId(jobId);
                setCandidates(data);
            };

            fetchCandidates();
        }
    }, [jobId]);

    return (
        <div>
            <h1>Candidate Portal</h1>
            <CandidateList candidates={candidates} />
        </div>
    );
};

export default CandidatePortal;