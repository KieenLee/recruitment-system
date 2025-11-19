import React, { useState } from "react";
import { Upload, Send, CheckCircle, AlertCircle } from "lucide-react";
import { candidateApi } from "../api/candidateService";
import "../css/UploadForm.css";

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

export default UploadForm;
