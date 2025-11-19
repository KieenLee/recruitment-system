import React, { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { jobPostingApi, candidateApi } from "../api/candidateService";
import {
  MapPin,
  Briefcase,
  DollarSign,
  Clock,
  ArrowLeft,
  Upload,
  CheckCircle,
} from "lucide-react";
import "../css/JobDetailPage.css";

const JobDetailPage = () => {
  const { jobId } = useParams();
  const navigate = useNavigate();
  const [job, setJob] = useState(null);
  const [loading, setLoading] = useState(true);
  const [applying, setApplying] = useState(false);
  const [applied, setApplied] = useState(false);
  const [formData, setFormData] = useState({
    fullName: "",
    email: "",
    phone: "",
  });
  const [cvFile, setCvFile] = useState(null);
  const [errors, setErrors] = useState({});

  useEffect(() => {
    fetchJobDetails();
  }, [jobId]);

  const fetchJobDetails = async () => {
    try {
      setLoading(true);
      const data = await jobPostingApi.getJobById(jobId);
      setJob(data);
    } catch (error) {
      console.error("Error fetching job:", error);
    } finally {
      setLoading(false);
    }
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
    // Clear error when user types
    if (errors[name]) {
      setErrors((prev) => ({ ...prev, [name]: "" }));
    }
  };

  const handleFileChange = (e) => {
    const file = e.target.files[0];
    if (file) {
      if (file.type !== "application/pdf") {
        setErrors((prev) => ({
          ...prev,
          cvFile: "Only PDF files are accepted",
        }));
        return;
      }
      if (file.size > 5 * 1024 * 1024) {
        // 5MB
        setErrors((prev) => ({
          ...prev,
          cvFile: "File size must be less than 5MB",
        }));
        return;
      }
      setCvFile(file);
      setErrors((prev) => ({ ...prev, cvFile: "" }));
    }
  };

  const validateForm = () => {
    const newErrors = {};
    if (!formData.fullName.trim()) newErrors.fullName = "Name is required";
    if (!formData.email.trim()) newErrors.email = "Email is required";
    if (!/\S+@\S+\.\S+/.test(formData.email))
      newErrors.email = "Email is invalid";
    if (!formData.phone.trim()) newErrors.phone = "Phone is required";
    if (!cvFile) newErrors.cvFile = "CV file is required";

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!validateForm()) return;

    try {
      setApplying(true);
      await candidateApi.applyToJob(jobId, formData, cvFile);
      setApplied(true);

      // Reset form
      setFormData({ fullName: "", email: "", phone: "" });
      setCvFile(null);

      // Auto close success message after 3s
      setTimeout(() => {
        navigate("/");
      }, 3000);
    } catch (error) {
      console.error("Error applying to job:", error);
      alert("Failed to submit application. Please try again.");
    } finally {
      setApplying(false);
    }
  };

  if (loading) {
    return (
      <div className="loading-container">
        <div className="spinner"></div>
        <p>Loading job details...</p>
      </div>
    );
  }

  if (!job) {
    return (
      <div className="error-container">
        <h2>Job not found</h2>
        <button onClick={() => navigate("/")}>Back to Jobs</button>
      </div>
    );
  }

  if (applied) {
    return (
      <div className="success-container">
        <CheckCircle size={64} className="success-icon" />
        <h2>Application Submitted!</h2>
        <p>Thank you for applying to {job.title}</p>
        <p>We'll review your application and get back to you soon.</p>
        <button onClick={() => navigate("/")}>Browse More Jobs</button>
      </div>
    );
  }

  return (
    <div className="job-detail-page">
      <div className="container">
        <button className="btn-back" onClick={() => navigate("/")}>
          <ArrowLeft size={20} />
          Back to Jobs
        </button>

        <div className="job-detail-layout">
          {/* Left Column - Job Info */}
          <div className="job-info-section">
            <div className="job-header">
              <div className="company-logo-large">
                {job.companyName?.charAt(0) || "C"}
              </div>
              <div>
                <h1 className="job-title">{job.title}</h1>
                <p className="company-name">{job.companyName}</p>
              </div>
            </div>

            <div className="job-meta">
              <div className="meta-item">
                <MapPin size={20} />
                <span>{job.location}</span>
              </div>
              <div className="meta-item">
                <Briefcase size={20} />
                <span>{job.employmentType}</span>
              </div>
              {(job.salaryMin || job.salaryMax) && (
                <div className="meta-item">
                  <DollarSign size={20} />
                  <span>
                    {job.salaryMin && job.salaryMax
                      ? `$${job.salaryMin} - $${job.salaryMax}`
                      : job.salaryMin
                      ? `From $${job.salaryMin}`
                      : `Up to $${job.salaryMax}`}
                  </span>
                </div>
              )}
              <div className="meta-item">
                <Clock size={20} />
                <span>
                  Posted {new Date(job.postedDate).toLocaleDateString()}
                </span>
              </div>
            </div>

            <div className="job-description">
              <h2>Job Description</h2>
              <div
                dangerouslySetInnerHTML={{
                  __html: job.description.replace(/\n/g, "<br />"),
                }}
              />
            </div>

            <div className="job-requirements">
              <h2>Requirements</h2>
              <div
                dangerouslySetInnerHTML={{
                  __html: job.requirements.replace(/\n/g, "<br />"),
                }}
              />
            </div>
          </div>

          {/* Right Column - Apply Form */}
          <div className="apply-form-section">
            <div className="apply-card">
              <h2>Apply for this position</h2>
              <form onSubmit={handleSubmit}>
                <div className="form-group">
                  <label>Full Name *</label>
                  <input
                    type="text"
                    name="fullName"
                    value={formData.fullName}
                    onChange={handleInputChange}
                    placeholder="John Doe"
                    className={errors.fullName ? "error" : ""}
                  />
                  {errors.fullName && (
                    <span className="error-message">{errors.fullName}</span>
                  )}
                </div>

                <div className="form-group">
                  <label>Email *</label>
                  <input
                    type="email"
                    name="email"
                    value={formData.email}
                    onChange={handleInputChange}
                    placeholder="john@example.com"
                    className={errors.email ? "error" : ""}
                  />
                  {errors.email && (
                    <span className="error-message">{errors.email}</span>
                  )}
                </div>

                <div className="form-group">
                  <label>Phone Number *</label>
                  <input
                    type="tel"
                    name="phone"
                    value={formData.phone}
                    onChange={handleInputChange}
                    placeholder="+84 123 456 789"
                    className={errors.phone ? "error" : ""}
                  />
                  {errors.phone && (
                    <span className="error-message">{errors.phone}</span>
                  )}
                </div>

                <div className="form-group">
                  <label>Upload CV (PDF only) *</label>
                  <div className="file-upload">
                    <input
                      type="file"
                      id="cv-upload"
                      accept=".pdf"
                      onChange={handleFileChange}
                      className="file-input"
                    />
                    <label htmlFor="cv-upload" className="file-label">
                      <Upload size={20} />
                      {cvFile ? cvFile.name : "Choose file..."}
                    </label>
                  </div>
                  {errors.cvFile && (
                    <span className="error-message">{errors.cvFile}</span>
                  )}
                  <small>Max file size: 5MB</small>
                </div>

                <button
                  type="submit"
                  className="btn-submit"
                  disabled={applying}
                >
                  {applying ? "Submitting..." : "Submit Application"}
                </button>
              </form>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default JobDetailPage;
