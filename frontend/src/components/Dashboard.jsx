import { useState, useEffect } from "react";
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
} from "lucide-react";
import { jobPostingApi } from "../api/candidateService";
import "../css/Dashboard.css";

const Dashboard = () => {
  const navigate = useNavigate();
  const [jobs, setJobs] = useState([]);
  const [filteredJobs, setFilteredJobs] = useState([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState("");
  const [statusFilter, setStatusFilter] = useState("All");

  useEffect(() => {
    fetchJobs();
  }, []);

  useEffect(() => {
    filterJobs();
  }, [jobs, searchTerm, statusFilter]);

  const fetchJobs = async () => {
    try {
      setLoading(true);
      const data = await jobPostingApi.getAllJobs();
      setJobs(data);
      setFilteredJobs(data);
    } catch (error) {
      console.error("Error fetching jobs:", error);
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

  if (loading) {
    return (
      <div className="loading-screen">
        <Loader className="spinner-large" size={48} />
        <p>Loading jobs...</p>
      </div>
    );
  }

  return (
    <div className="dashboard-container">
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
