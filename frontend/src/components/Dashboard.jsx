import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import {
  Briefcase,
  MapPin,
  Calendar,
  Users,
  Plus,
  Search,
  Filter,
  Loader,
  Building2,
  DollarSign,
  CheckCircle,
  Clock,
  TrendingUp,
} from "lucide-react";
import { jobPostingApi, candidateApi } from "../api/candidateService";
import "../css/Dashboard.css";

const Dashboard = () => {
  const navigate = useNavigate();
  const [jobs, setJobs] = useState([]);
  const [filteredJobs, setFilteredJobs] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [searchTerm, setSearchTerm] = useState("");
  const [statusFilter, setStatusFilter] = useState("All");
  const [stats, setStats] = useState({
    totalJobs: 0,
    activeJobs: 0,
    totalCandidates: 0,
    pendingReviews: 0,
  });
  const [recentJobs, setRecentJobs] = useState([]);
  const [recentCandidates, setRecentCandidates] = useState([]);

  useEffect(() => {
    fetchDashboardData();
  }, []);

  useEffect(() => {
    filterJobs();
  }, [jobs, searchTerm, statusFilter]);

  const fetchDashboardData = async () => {
    try {
      setLoading(true);

      // Fetch all jobs
      const jobs = await jobPostingApi.getAllJobs();

      // Calculate stats
      const activeJobs = jobs.filter((job) => job.status === "Open");

      // Fetch candidates for all active jobs
      let allCandidates = [];
      for (const job of activeJobs) {
        try {
          const candidates = await candidateApi.getCandidatesByJobId(job.id);
          allCandidates = [...allCandidates, ...candidates];
        } catch (err) {
          console.error(`Error fetching candidates for job ${job.id}:`, err);
        }
      }

      const pendingCandidates = allCandidates.filter(
        (c) => c.status === "Pending"
      );

      setStats({
        totalJobs: jobs.length,
        activeJobs: activeJobs.length,
        totalCandidates: allCandidates.length,
        pendingReviews: pendingCandidates.length,
      });

      // Set recent jobs (top 5)
      setRecentJobs(jobs.slice(0, 5));

      // Set recent candidates (top 5)
      setRecentCandidates(allCandidates.slice(0, 5));
    } catch (err) {
      console.error("Error fetching dashboard data:", err);
      setError(err.message || "Failed to load dashboard data");
    } finally {
      setLoading(false);
    }
  };

  const filterJobs = () => {
    let filtered = [...jobs];

    // Filter by status
    if (statusFilter !== "All") {
      filtered = filtered.filter((job) => job.status === statusFilter);
    }

    // Filter by search term
    if (searchTerm.trim()) {
      filtered = filtered.filter(
        (job) =>
          job.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
          job.companyName.toLowerCase().includes(searchTerm.toLowerCase()) ||
          job.location.toLowerCase().includes(searchTerm.toLowerCase())
      );
    }

    setFilteredJobs(filtered);
  };

  const getStatusColor = (status) => {
    switch (status.toLowerCase()) {
      case "open":
        return "status-open";
      case "closed":
        return "status-closed";
      default:
        return "status-draft";
    }
  };

  // ✅ Safe date formatter
  const formatDate = (dateString) => {
    if (!dateString) return "N/A";
    try {
      return new Date(dateString).toLocaleDateString("en-US", {
        year: "numeric",
        month: "short",
        day: "numeric",
      });
    } catch (err) {
      return "Invalid Date";
    }
  };

  // ✅ Safe string truncate
  const truncateText = (text, maxLength = 100) => {
    if (!text) return "";
    if (typeof text !== "string") return String(text);
    return text.length > maxLength
      ? text.substring(0, maxLength) + "..."
      : text;
  };

  // ✅ Safe score formatter
  const formatScore = (score) => {
    if (score === undefined || score === null) return "N/A";
    if (typeof score === "number") return score.toFixed(1);
    return String(score);
  };

  if (loading) {
    return (
      <div className="loading-screen">
        <Loader className="spinner-large" size={48} />
        <p>Loading jobs...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="dashboard-error">
        <p>Error: {error}</p>
        <button onClick={fetchDashboardData}>Retry</button>
      </div>
    );
  }

  return (
    <div className="dashboard-container">
      <div className="dashboard-header">
        <h1>Recruitment Dashboard</h1>
        <button className="refresh-btn" onClick={fetchDashboardData}>
          <TrendingUp size={20} />
          Refresh
        </button>
      </div>

      {/* Stats Cards */}
      <div className="stats-grid">
        <div className="stat-card">
          <div className="stat-icon briefcase">
            <Briefcase size={24} />
          </div>
          <div className="stat-info">
            <h3>{stats.totalJobs}</h3>
            <p>Total Job Postings</p>
          </div>
        </div>

        <div className="stat-card">
          <div className="stat-icon active">
            <CheckCircle size={24} />
          </div>
          <div className="stat-info">
            <h3>{stats.activeJobs}</h3>
            <p>Active Jobs</p>
          </div>
        </div>

        <div className="stat-card">
          <div className="stat-icon candidates">
            <Users size={24} />
          </div>
          <div className="stat-info">
            <h3>{stats.totalCandidates}</h3>
            <p>Total Candidates</p>
          </div>
        </div>

        <div className="stat-card">
          <div className="stat-icon pending">
            <Clock size={24} />
          </div>
          <div className="stat-info">
            <h3>{stats.pendingReviews}</h3>
            <p>Pending Reviews</p>
          </div>
        </div>
      </div>

      {/* Recent Jobs Section */}
      <div className="dashboard-section">
        <h2>Recent Job Postings</h2>
        {recentJobs.length === 0 ? (
          <p className="empty-message">No job postings yet</p>
        ) : (
          <div className="recent-jobs-list">
            {recentJobs.map((job) => (
              <div key={job.id} className="job-item">
                <div className="job-header">
                  <h3>{job.title || "Untitled Job"}</h3>
                  <span
                    className={`status-badge status-${(
                      job.status || ""
                    ).toLowerCase()}`}
                  >
                    {job.status || "Unknown"}
                  </span>
                </div>
                <p className="job-company">
                  {job.companyName || "Unknown Company"}
                </p>
                <p className="job-location">
                  📍 {job.location || "Location not specified"}
                </p>
                <div className="job-meta">
                  <span>📅 {formatDate(job.postedDate)}</span>
                  <span>👥 {job.totalCandidates || 0} candidates</span>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>

      {/* Recent Candidates Section */}
      <div className="dashboard-section">
        <h2>Recent Candidates</h2>
        {recentCandidates.length === 0 ? (
          <p className="empty-message">No candidates yet</p>
        ) : (
          <div className="recent-candidates-list">
            {recentCandidates.map((candidate) => (
              <div key={candidate.id} className="candidate-item">
                <div className="candidate-header">
                  <div className="candidate-info">
                    <h4>{candidate.fullName || "Unknown Candidate"}</h4>
                    <p className="candidate-email">
                      {candidate.email || "No email"}
                    </p>
                  </div>
                  <span
                    className={`status-badge status-${(
                      candidate.status || "pending"
                    ).toLowerCase()}`}
                  >
                    {candidate.status || "Pending"}
                  </span>
                </div>

                {/* ✅ Safe rendering với optional chaining và fallbacks */}
                {candidate.aiAnalysis && (
                  <div className="ai-preview">
                    <div className="score-badge">
                      Score: {formatScore(candidate.aiAnalysis.overallScore)}/10
                    </div>
                    <p className="ai-summary">
                      {truncateText(candidate.aiAnalysis.summary, 150)}
                    </p>
                  </div>
                )}

                <div className="candidate-meta">
                  <span>📅 Applied: {formatDate(candidate.uploadedAt)}</span>
                  {candidate.phone && <span>📞 {candidate.phone}</span>}
                </div>
              </div>
            ))}
          </div>
        )}
      </div>

      {/* Header */}
      <div className="dashboard-header">
        <div className="header-content">
          <div className="header-left">
            <h1>Job Postings</h1>
            <p>Manage and track all your job postings</p>
          </div>
          <button
            className="btn-create-job"
            onClick={() => navigate("/jobs/create")}
          >
            <Plus size={20} />
            Create New Job
          </button>
        </div>

        {/* Search and Filter Bar */}
        <div className="search-filter-bar">
          <div className="search-box">
            <Search size={20} className="search-icon" />
            <input
              type="text"
              placeholder="Search by title, company, or location..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
            />
          </div>

          <div className="filter-group">
            <Filter size={20} />
            <select
              value={statusFilter}
              onChange={(e) => setStatusFilter(e.target.value)}
              className="filter-select"
            >
              <option value="All">All Status</option>
              <option value="Open">Open</option>
              <option value="Closed">Closed</option>
              <option value="Draft">Draft</option>
            </select>
          </div>
        </div>
      </div>

      {/* Stats Overview */}
      <div className="stats-overview">
        <div className="stat-box">
          <div className="stat-icon total-jobs">
            <Briefcase size={24} />
          </div>
          <div className="stat-info">
            <h3>{jobs.length}</h3>
            <p>Total Jobs</p>
          </div>
        </div>

        <div className="stat-box">
          <div className="stat-icon open-jobs">
            <Briefcase size={24} />
          </div>
          <div className="stat-info">
            <h3>{jobs.filter((j) => j.status === "Open").length}</h3>
            <p>Open Positions</p>
          </div>
        </div>

        <div className="stat-box">
          <div className="stat-icon total-candidates">
            <Users size={24} />
          </div>
          <div className="stat-info">
            <h3>{jobs.reduce((sum, job) => sum + job.totalCandidates, 0)}</h3>
            <p>Total Candidates</p>
          </div>
        </div>

        <div className="stat-box">
          <div className="stat-icon avg-candidates">
            <Users size={24} />
          </div>
          <div className="stat-info">
            <h3>
              {jobs.length > 0
                ? Math.round(
                    jobs.reduce((sum, job) => sum + job.totalCandidates, 0) /
                      jobs.length
                  )
                : 0}
            </h3>
            <p>Avg per Job</p>
          </div>
        </div>
      </div>

      {/* Jobs Grid */}
      {filteredJobs.length === 0 ? (
        <div className="empty-state-dashboard">
          <Briefcase size={64} className="empty-icon" />
          <h2>No Jobs Found</h2>
          <p>
            {searchTerm || statusFilter !== "All"
              ? "Try adjusting your search or filters"
              : "Get started by creating your first job posting"}
          </p>
          {!searchTerm && statusFilter === "All" && (
            <button
              className="btn-primary-large"
              onClick={() => navigate("/jobs/create")}
            >
              <Plus size={20} />
              Create First Job
            </button>
          )}
        </div>
      ) : (
        <div className="jobs-grid">
          {filteredJobs.map((job) => (
            <div
              key={job.id}
              className="job-card"
              onClick={() => navigate(`/jobs/${job.id}/dashboard`)}
            >
              <div className="job-card-header">
                <div className="job-title-row">
                  <h3>{job.title}</h3>
                  <span
                    className={`status-badge ${getStatusColor(job.status)}`}
                  >
                    {job.status}
                  </span>
                </div>

                <div className="job-company">
                  <Building2 size={16} />
                  <span>{job.companyName}</span>
                </div>
              </div>

              <div className="job-card-body">
                <div className="job-meta-row">
                  <div className="meta-item">
                    <MapPin size={16} />
                    <span>{job.location}</span>
                  </div>
                  <div className="meta-item">
                    <Briefcase size={16} />
                    <span>{job.employmentType}</span>
                  </div>
                </div>

                {job.salaryMin > 0 && (
                  <div className="job-salary">
                    <DollarSign size={16} />
                    <span>
                      ${job.salaryMin.toLocaleString()} - $
                      {job.salaryMax.toLocaleString()}
                    </span>
                  </div>
                )}

                <div className="job-description-preview">
                  {job.description.substring(0, 120)}
                  {job.description.length > 120 && "..."}
                </div>
              </div>

              <div className="job-card-footer">
                <div className="job-date">
                  <Calendar size={16} />
                  <span>
                    Posted {new Date(job.postedDate).toLocaleDateString()}
                  </span>
                </div>

                <div className="job-candidates">
                  <Users size={16} />
                  <span>
                    <strong>{job.totalCandidates}</strong> Candidate
                    {job.totalCandidates !== 1 ? "s" : ""}
                  </span>
                </div>
              </div>

              <div className="job-card-actions">
                <button
                  className="btn-view-candidates"
                  onClick={(e) => {
                    e.stopPropagation();
                    navigate(`/jobs/${job.id}/dashboard`);
                  }}
                >
                  View Dashboard
                </button>
                <button
                  className="btn-copy-link"
                  onClick={(e) => {
                    e.stopPropagation();
                    const link = `${window.location.origin}/apply/${job.id}`;
                    navigator.clipboard.writeText(link);
                    alert("Application link copied to clipboard!");
                  }}
                >
                  Copy Link
                </button>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default Dashboard;
