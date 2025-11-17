import React, { useEffect, useState } from 'react';
import { getCandidatesByJobId } from '../api/candidateService';

const CandidateList = ({ jobId }) => {
    const [candidates, setCandidates] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        const fetchCandidates = async () => {
            try {
                const data = await getCandidatesByJobId(jobId);
                setCandidates(data);
            } catch (err) {
                setError(err.message);
            } finally {
                setLoading(false);
            }
        };

        fetchCandidates();
    }, [jobId]);

    if (loading) {
        return <div>Loading candidates...</div>;
    }

    if (error) {
        return <div>Error fetching candidates: {error}</div>;
    }

    return (
        <div>
            <h2>Candidates for Job ID: {jobId}</h2>
            <ul>
                {candidates.map(candidate => (
                    <li key={candidate._id}>
                        <strong>Name:</strong> {candidate.Name} <br />
                        <strong>Email:</strong> {candidate.Email} <br />
                        <strong>Status:</strong> {candidate.Status} <br />
                        <strong>CV File:</strong> {candidate.CVFileName} <br />
                    </li>
                ))}
            </ul>
        </div>
    );
};

export default CandidateList;