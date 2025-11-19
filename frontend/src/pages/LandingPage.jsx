import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { jobPostingApi } from "../api/candidateService";
import {
  Search,
  MapPin,
  Briefcase,
  DollarSign,
  Clock,
  ArrowRight,
} from "lucide-react";
import "../css/LandingPage.css";

const LandingPage = () => {
  const navigate = useNavigate();
  const [jobs, setJobs] = useState([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState("");
  const [locationFilter, setLocationFilter] = useState("");
  const [employmentTypeFilter, setEmploymentTypeFilter] = useState("");

  useEffect(() => {
    fetchJobs();
  }, []);

  const fetchJobs = async () => {
    try {
      setLoading(true);
      const data = await jobPostingApi.getAllJobs("Open"); // Only open jobs
      setJobs(data);
    } catch (error) {
      console.error("Error fetching jobs:", error);
    } finally {
      setLoading(false);
    }
  };

  const filteredJobs = jobs.filter((job) => {
    const matchesSearch =
      job.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
      job.description?.toLowerCase().includes(searchTerm.toLowerCase());
    const matchesLocation = !locationFilter || job.location === locationFilter;
    const matchesType =
      !employmentTypeFilter || job.employmentType === employmentTypeFilter;
    return matchesSearch && matchesLocation && matchesType;
  });

  const locations = [...new Set(jobs.map((job) => job.location))];
  const employmentTypes = [...new Set(jobs.map((job) => job.employmentType))];

  const formatSalary = (min, max) => {
    if (!min && !max) return "Competitive";
    if (min && max) return `$${min} - $${max}`;
    if (min) return `From $${min}`;
    if (max) return `Up to $${max}`;
  };

  const formatDate = (dateString) => {
    const date = new Date(dateString);
    const now = new Date();
    const diffTime = Math.abs(now - date);
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));

    if (diffDays === 0) return "Today";
    if (diffDays === 1) return "Yesterday";
    if (diffDays < 7) return `${diffDays} days ago`;
    if (diffDays < 30) return `${Math.floor(diffDays / 7)} weeks ago`;
    return date.toLocaleDateString();
  };

  return (
    <div className="landing-page">
      {/* Header */}
      <header className="landing-header">
        <div className="container">
          <div className="header-content">
            <h1 className="logo">🎯 TalentHub</h1>
            <nav className="nav-links">
              <a href="#jobs">Jobs</a>
              <a href="#about">About</a>
              <a href="#contact">Contact</a>
              <button className="btn-login" onClick={() => navigate("/login")}>
                HR Login
              </button>
            </nav>
          </div>
        </div>
      </header>

      {/* Hero Section */}
      <section className="hero-section">
        <div className="container">
          <div className="hero-content">
            <h1 className="hero-title">Find Your Dream Job Today</h1>
            <p className="hero-subtitle">
              Discover thousands of opportunities from top companies
            </p>

            {/* Search Bar */}
            <div className="search-box">
              <div className="search-input-group">
                <Search size={20} className="search-icon" />
                <input
                  type="text"
                  placeholder="Job title, keywords..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  className="search-input"
                />
              </div>
              <button className="btn-search" onClick={fetchJobs}>
                Search Jobs
              </button>
            </div>

            {/* Quick Stats */}
            <div className="quick-stats">
              <div className="stat-item">
                <strong>{jobs.length}</strong> Active Jobs
              </div>
              <div className="stat-item">
                <strong>
                  {[...new Set(jobs.map((j) => j.companyName))].length}
                </strong>{" "}
                Companies
              </div>
              <div className="stat-item">
                <strong>{locations.length}</strong> Locations
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* Filters */}
      <section className="filters-section">
        <div className="container">
          <div className="filters-bar">
            <select
              value={locationFilter}
              onChange={(e) => setLocationFilter(e.target.value)}
              className="filter-select"
            >
              <option value="">All Locations</option>
              {locations.map((loc) => (
                <option key={loc} value={loc}>
                  {loc}
                </option>
              ))}
            </select>

            <select
              value={employmentTypeFilter}
              onChange={(e) => setEmploymentTypeFilter(e.target.value)}
              className="filter-select"
            >
              <option value="">All Types</option>
              {employmentTypes.map((type) => (
                <option key={type} value={type}>
                  {type}
                </option>
              ))}
            </select>

            <button
              className="btn-clear-filters"
              onClick={() => {
                setSearchTerm("");
                setLocationFilter("");
                setEmploymentTypeFilter("");
              }}
            >
              Clear Filters
            </button>
          </div>
        </div>
      </section>

      {/* Jobs List */}
      <section className="jobs-section" id="jobs">
        <div className="container">
          <h2 className="section-title">
            {filteredJobs.length} Jobs Available
          </h2>

          {loading ? (
            <div className="loading-state">
              <div className="spinner"></div>
              <p>Loading jobs...</p>
            </div>
          ) : filteredJobs.length === 0 ? (
            <div className="empty-state">
              <Briefcase size={48} />
              <h3>No jobs found</h3>
              <p>Try adjusting your search criteria</p>
            </div>
          ) : (
            <div className="jobs-grid">
              {filteredJobs.map((job) => (
                <div
                  key={job.id}
                  className="job-card"
                  onClick={() => navigate(`/jobs/${job.id}`)}
                >
                  <div className="job-card-header">
                    <div className="company-logo">
                      {job.companyName.charAt(0)}
                    </div>
                    <div className="job-title-section">
                      <h3 className="job-title">{job.title}</h3>
                      <p className="company-name">{job.companyName}</p>
                    </div>
                  </div>

                  <div className="job-details">
                    <div className="job-detail-item">
                      <MapPin size={16} />
                      <span>{job.location}</span>
                    </div>
                    <div className="job-detail-item">
                      <Briefcase size={16} />
                      <span>{job.employmentType}</span>
                    </div>
                    <div className="job-detail-item">
                      <DollarSign size={16} />
                      <span>{formatSalary(job.salaryMin, job.salaryMax)}</span>
                    </div>
                    <div className="job-detail-item">
                      <Clock size={16} />
                      <span>{formatDate(job.postedDate)}</span>
                    </div>
                  </div>

                  {job.totalCandidates > 0 && (
                    <div className="applicants-badge">
                      {job.totalCandidates} applicants
                    </div>
                  )}

                  <button className="btn-view-job">
                    View Details
                    <ArrowRight size={16} />
                  </button>
                </div>
              ))}
            </div>
          )}
        </div>
      </section>

      {/* Footer */}
      <footer className="landing-footer">
        <div className="container">
          <div className="footer-content">
            <div className="footer-section">
              <h3>TalentHub</h3>
              <p>Your gateway to career success</p>
            </div>
            <div className="footer-section">
              <h4>For Job Seekers</h4>
              <a href="#jobs">Browse Jobs</a>
              <a href="#companies">Companies</a>
              <a href="#resources">Career Resources</a>
            </div>
            <div className="footer-section">
              <h4>For Employers</h4>
              <a href="/login">Post a Job</a>
              <a href="/login">Manage Candidates</a>
              <a href="/login">HR Dashboard</a>
            </div>
            <div className="footer-section">
              <h4>Company</h4>
              <a href="#about">About Us</a>
              <a href="#contact">Contact</a>
              <a href="#privacy">Privacy Policy</a>
            </div>
          </div>
          <div className="footer-bottom">
            <p>&copy; 2025 TalentHub. All rights reserved.</p>
          </div>
        </div>
      </footer>
    </div>
  );
};

export default LandingPage;
