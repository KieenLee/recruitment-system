import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { jobPostingApi } from "../api/candidateService";
import {
  Plus,
  Search,
  MapPin,
  Briefcase,
  DollarSign,
  Calendar,
  Users,
  Edit,
  Trash2,
  Eye,
  MoreVertical,
} from "lucide-react";
import "../css/JobList.css";

const JobList = () => {
  const navigate = useNavigate();
  const [jobs, setJobs] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [searchTerm, setSearchTerm] = useState("");
  const [statusFilter, setStatusFilter] = useState("All");
  const [showCreateModal, setShowCreateModal] = useState(false);

  useEffect(() => {
    fetchJobs();
  }, []);

  const fetchJobs = async () => {
    try {
      setLoading(true);
      const data = await jobPostingApi.getAllJobs();
      setJobs(data);
      setError(null);
    } catch (err) {
      console.error("Error fetching jobs:", err);
      setError(err.message || "Failed to fetch jobs");
    } finally {
      setLoading(false);
    }
  };

  const handleCreateJob = () => {
    setShowCreateModal(true);
    // TODO: Implement create job modal
  };

  const handleEditJob = (jobId) => {
    // TODO: Implement edit job functionality
    console.log("Edit job:", jobId);
  };

  const handleDeleteJob = async (jobId) => {
    if (!window.confirm("Are you sure you want to delete this job posting?")) {
      return;
    }

    try {
      await jobPostingApi.deleteJob(jobId);
      // Refresh jobs list
      fetchJobs();
    } catch (err) {
      console.error("Error deleting job:", err);
      alert("Failed to delete job posting");
    }
  };

  const handleViewCandidates = (jobId) => {
    navigate(`/hr/jobs/${jobId}/candidates`);
  };

  const filteredJobs = jobs.filter((job) => {
    const matchesSearch =
      job.title?.toLowerCase().includes(searchTerm.toLowerCase()) ||
      job.companyName?.toLowerCase().includes(searchTerm.toLowerCase()) ||
      job.location?.toLowerCase().includes(searchTerm.toLowerCase());
    const matchesStatus = statusFilter === "All" || job.status === statusFilter;
    return matchesSearch && matchesStatus;
  });

  const getStatusColor = (status) => {
    const statusMap = {
      Open: "success",
      Closed: "danger",
      Draft: "warning",
    };
    return statusMap[status] || "default";
  };

  const formatDate = (dateString) => {
    if (!dateString) return "N/A";
    try {
      return new Date(dateString).toLocaleDateString("en-US", {
        year: "numeric",
        month: "short",
        day: "numeric",
      });
    } catch {
      return "Invalid Date";
    }
  };

  const formatSalary = (min, max) => {
    if (!min && !max) return "Competitive";
    if (min && max) return `$${min} - $${max}`;
    if (min) return `From $${min}`;
    if (max) return `Up to $${max}`;
  };

  if (loading) {
    return (
      <div className="job-list-loading">
        <div className="spinner"></div>
        <p>Loading jobs...</p>
      </div>
    );
  }

  return (
    <div className="job-list-container">
      {/* Header */}
      <div className="job-list-header">
        <div>
          <h1>Job Postings</h1>
          <p className="subtitle">Manage all job openings and postings</p>
        </div>
        <button className="btn-create-job" onClick={handleCreateJob}>
          <Plus size={20} />
          Create Job
        </button>
      </div>

      {/* Filters */}
      <div className="job-filters">
        <div className="search-box">
          <Search size={20} className="search-icon" />
          <input
            type="text"
            placeholder="Search jobs by title, company, or location..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="search-input"
          />
        </div>

        <div className="filter-tabs">
          {["All", "Open", "Closed", "Draft"].map((status) => (
            <button
              key={status}
              className={`filter-tab ${
                statusFilter === status ? "active" : ""
              }`}
              onClick={() => setStatusFilter(status)}
            >
              {status}
              <span className="count">
                {status === "All"
                  ? jobs.length
                  : jobs.filter((j) => j.status === status).length}
              </span>
            </button>
          ))}
        </div>
      </div>

      {/* Error State */}
      {error && (
        <div className="error-banner">
          <p>{error}</p>
          <button onClick={fetchJobs}>Retry</button>
        </div>
      )}

      {/* Jobs List */}
      {filteredJobs.length === 0 ? (
        <div className="empty-state">
          <Briefcase size={64} />
          <h3>No jobs found</h3>
          <p>
            {searchTerm || statusFilter !== "All"
              ? "Try adjusting your filters"
              : "Get started by creating your first job posting"}
          </p>
          {!searchTerm && statusFilter === "All" && (
            <button className="btn-create-job-empty" onClick={handleCreateJob}>
              <Plus size={20} />
              Create Job Posting
            </button>
          )}
        </div>
      ) : (
        <div className="jobs-grid">
          {filteredJobs.map((job) => (
            <div key={job.id} className="job-card">
              {/* Job Header */}
              <div className="job-card-header">
                <div className="job-title-section">
                  <h3>{job.title}</h3>
                  <p className="company-name">{job.companyName}</p>
                </div>
                <span
                  className={`status-badge status-${getStatusColor(
                    job.status
                  )}`}
                >
                  {job.status}
                </span>
              </div>

              {/* Job Details */}
              <div className="job-details">
                <div className="detail-row">
                  <MapPin size={16} />
                  <span>{job.location || "Remote"}</span>
                </div>
                <div className="detail-row">
                  <Briefcase size={16} />
                  <span>{job.employmentType || "Full-time"}</span>
                </div>
                <div className="detail-row">
                  <DollarSign size={16} />
                  <span>{formatSalary(job.salaryMin, job.salaryMax)}</span>
                </div>
                <div className="detail-row">
                  <Calendar size={16} />
                  <span>Posted {formatDate(job.postedDate)}</span>
                </div>
              </div>

              {/* Candidates Info */}
              <div className="candidates-info">
                <Users size={18} />
                <span>
                  {job.totalCandidates || 0}{" "}
                  {job.totalCandidates === 1 ? "candidate" : "candidates"}
                </span>
                {job.totalCandidates > 0 && (
                  <button
                    className="btn-view-candidates"
                    onClick={() => handleViewCandidates(job.id)}
                  >
                    View
                  </button>
                )}
              </div>

              {/* Actions */}
              <div className="job-actions">
                <button
                  className="btn-action btn-view"
                  onClick={() => navigate(`/jobs/${job.id}`)}
                  title="View Details"
                >
                  <Eye size={18} />
                  View
                </button>
                <button
                  className="btn-action btn-edit"
                  onClick={() => handleEditJob(job.id)}
                  title="Edit Job"
                >
                  <Edit size={18} />
                  Edit
                </button>
                <button
                  className="btn-action btn-delete"
                  onClick={() => handleDeleteJob(job.id)}
                  title="Delete Job"
                >
                  <Trash2 size={18} />
                  Delete
                </button>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default JobList;
