import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { jobPostingApi } from "../api/candidateService";
import UploadForm from "../components/UploadForm";
import "../css/UploadForm.css";
import '../css/CandidatePortal.css';

const CandidatePortal = () => {
  const { jobId } = useParams();
  const [job, setJob] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    const fetchJob = async () => {
      try {
        setLoading(true);
        const jobData = await jobPostingApi.getJobById(jobId);
        setJob(jobData);
      } catch (err) {
        setError("Failed to load job details");
        console.error(err);
      } finally {
        setLoading(false);
      }
    };

    if (jobId) {
      fetchJob();
    }
  }, [jobId]);

  if (loading) {
    return (
      <div className="loading-container">
        <div className="spinner-large"></div>
        <p>Loading job details...</p>
      </div>
    );
  }

  if (error || !job) {
    return (
      <div className="error-container">
        <h2>Job Not Found</h2>
        <p>{error || "The job posting you are looking for does not exist."}</p>
      </div>
    );
  }

  return (
    <div className="candidate-portal">
      <div className="job-details-card">
        <h1>{job.title}</h1>
        <div className="job-meta">
          <span className="company-name">{job.companyName}</span>
          <span className="location">{job.location}</span>
          <span className="employment-type">{job.employmentType}</span>
        </div>
        <div className="job-description">
          <h3>Job Description</h3>
          <p>{job.description}</p>
        </div>
        <div className="job-requirements">
          <h3>Requirements</h3>
          <p>{job.requirements}</p>
        </div>
        {job.salaryMin > 0 && (
          <div className="salary-range">
            <h3>Salary Range</h3>
            <p>
              ${job.salaryMin.toLocaleString()} - $
              {job.salaryMax.toLocaleString()}
            </p>
          </div>
        )}
      </div>

      <UploadForm jobId={jobId} jobTitle={job.title} />
    </div>
  );
};

export default CandidatePortal;
