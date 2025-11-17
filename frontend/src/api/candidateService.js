import axios from 'axios';

const API_BASE_URL = 'http://localhost:5000/api/jobs';

export const applyForJob = async (jobId, formData) => {
    try {
        const response = await axios.post(`${API_BASE_URL}/${jobId}/apply`, formData, {
            headers: {
                'Content-Type': 'multipart/form-data',
            },
        });
        return response.data;
    } catch (error) {
        console.error('Error applying for job:', error);
        throw error;
    }
};

export const getCandidatesForJob = async (jobId) => {
    try {
        const response = await axios.get(`${API_BASE_URL}/${jobId}/candidates`);
        return response.data;
    } catch (error) {
        console.error('Error fetching candidates:', error);
        throw error;
    }
};