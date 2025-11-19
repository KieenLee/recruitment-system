import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { getCandidatesByJobId, jobPostingApi } from "../api/candidateService";
import CandidateList from "../components/CandidateList";
import UploadForm from "../components/UploadForm";
import { Upload, Send, CheckCircle, AlertCircle } from "lucide-react";
import "./UploadForm.css";

const CandidatePortal = () => {
  const { jobId } = useParams();
  const [candidates, setCandidates] = useState([]);
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

    const fetchCandidates = async () => {
      const data = await getCandidatesByJobId(jobId);
      setCandidates(data);
    };

    if (jobId) {
      fetchJob();
      fetchCandidates();
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

const UploadForm = ({ jobId, jobTitle }) => {
  const [formData, setFormData] = useState({
    fullName: "",
    email: "",
    phone: "",
  });
  const [cvFile, setCvFile] = useState(null);
  const [loading, setLoading] = useState(false);
  const [success, setSuccess] = useState(false);
  const [error, setError] = useState("");

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleFileChange = (e) => {
    const file = e.target.files[0];
    if (file) {
      if (file.type !== "application/pdf") {
        setError("Only PDF files are accepted");
        setCvFile(null);
        return;
      }
      if (file.size > 5 * 1024 * 1024) {
        setError("File size must be less than 5MB");
        setCvFile(null);
        return;
      }
      setCvFile(file);
      setError("");
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");
    setSuccess(false);

    // Validation
    if (!formData.fullName || !formData.email || !formData.phone) {
      setError("Please fill in all fields");
      return;
    }
    if (!cvFile) {
      setError("Please upload your CV (PDF only)");
      return;
    }

    setLoading(true);

    try {
      await candidateApi.applyToJob(jobId, formData, cvFile);
      setSuccess(true);
      setFormData({ fullName: "", email: "", phone: "" });
      setCvFile(null);
      // Reset file input
      document.getElementById("cv-file-input").value = "";
    } catch (err) {
      setError(
        err.response?.data?.message ||
          "Failed to submit application. Please try again."
      );
    } finally {
      setLoading(false);
    }
  };

  if (success) {
    return (
      <div className="success-message">
        <CheckCircle size={64} className="success-icon" />
        <h2>Application Submitted Successfully!</h2>
        <p>Thank you for applying to {jobTitle}.</p>
        <p>We will review your CV and contact you soon.</p>
        <button onClick={() => setSuccess(false)} className="btn-primary">
          Submit Another Application
        </button>
      </div>
    );
  }

  return (
    <div className="upload-form-container">
      <div className="upload-form-header">
        <h2>Apply for: {jobTitle}</h2>
        <p>Fill in your information and upload your CV (PDF only)</p>
      </div>

      <form onSubmit={handleSubmit} className="upload-form">
        {error && (
          <div className="alert alert-error">
            <AlertCircle size={20} />
            <span>{error}</span>
          </div>
        )}

        <div className="form-group">
          <label htmlFor="fullName">Full Name *</label>
          <input
            type="text"
            id="fullName"
            name="fullName"
            value={formData.fullName}
            onChange={handleInputChange}
            placeholder="Enter your full name"
            required
          />
        </div>

        <div className="form-group">
          <label htmlFor="email">Email Address *</label>
          <input
            type="email"
            id="email"
            name="email"
            value={formData.email}
            onChange={handleInputChange}
            placeholder="your.email@example.com"
            required
          />
        </div>

        <div className="form-group">
          <label htmlFor="phone">Phone Number *</label>
          <input
            type="tel"
            id="phone"
            name="phone"
            value={formData.phone}
            onChange={handleInputChange}
            placeholder="+1234567890"
            required
          />
        </div>

        <div className="form-group">
          <label htmlFor="cv-file-input">Upload CV (PDF only) *</label>
          <div className="file-upload-wrapper">
            <input
              type="file"
              id="cv-file-input"
              accept=".pdf"
              onChange={handleFileChange}
              required
            />
            <div className="file-upload-display">
              <Upload size={24} />
              <span>{cvFile ? cvFile.name : "Choose a PDF file"}</span>
            </div>
          </div>
        </div>

        <button type="submit" className="btn-submit" disabled={loading}>
          {loading ? (
            <>
              <div className="spinner"></div>
              Submitting...
            </>
          ) : (
            <>
              <Send size={20} />
              Submit Application
            </>
          )}
        </button>
      </form>
    </div>
  );
};
