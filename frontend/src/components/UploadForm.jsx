import React, { useState } from 'react';
import { uploadCv } from '../api/candidateService';

const UploadForm = ({ jobId }) => {
    const [file, setFile] = useState(null);
    const [status, setStatus] = useState('');

    const handleFileChange = (event) => {
        setFile(event.target.files[0]);
    };

    const handleSubmit = async (event) => {
        event.preventDefault();
        if (!file) {
            setStatus('Please select a file to upload.');
            return;
        }

        const formData = new FormData();
        formData.append('cv', file);

        try {
            await uploadCv(jobId, formData);
            setStatus('CV uploaded successfully!');
        } catch (error) {
            setStatus('Error uploading CV. Please try again.');
        }
    };

    return (
        <div>
            <h2>Upload CV</h2>
            <form onSubmit={handleSubmit}>
                <input type="file" onChange={handleFileChange} />
                <button type="submit">Upload</button>
            </form>
            {status && <p>{status}</p>}
        </div>
    );
};

export default UploadForm;