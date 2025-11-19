import React, { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import {
  ArrowLeft,
  Briefcase,
  Users,
  Calendar,
  DollarSign,
  MapPin,
  Building,
  TrendingUp,
  Loader,
} from "lucide-react";
import { jobPostingApi, candidateApi } from "../api/candidateService";
import CandidateList from "../components/CandidateList";
import CandidateDetailsModal from "../components/CandidateDetailsModal";
import "../css/HrDashboard.css";

const HrDashboard = () => {
  const { jobId } = useParams();
  const navigate = useNavigate();

  const [job, setJob] = useState(null);
  const [candidates, setCandidates] = useState([]);
  const [selectedCandidate, setSelectedCandidate] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [showModal, setShowModal] = useState(false);

  useEffect(() => {
    fetchJobAndCandidates();
  }, [jobId]);

  const fetchJobAndCandidates = async () => {
    try {
      setLoading(true);
      setError("");

      // Fetch job details
      const jobData = await jobPostingApi.getJobById(jobId);
      setJob(jobData);

      // Fetch candidates
      const candidatesData = await candidateApi.getCandidatesByJobId(jobId);
      setCandidates(candidatesData);
    } catch (err) {
      console.error("Error fetching data:", err);
      setError("Failed to load dashboard data");
    } finally {
      setLoading(false);
    }
  };

  const handleStatusUpdate = async (candidateId, newStatus) => {
    try {
      await candidateApi.updateCandidateStatus(jobId, candidateId, newStatus);

      // Update local state
      setCandidates((prevCandidates) =>
        prevCandidates.map((c) =>
          c.id === candidateId ? { ...c, status: newStatus } : c
        )
      );

      // Show success notification (you can implement a toast notification)
      alert(`Candidate status updated to ${newStatus}`);
    } catch (err) {
      console.error("Error updating status:", err);
      alert("Failed to update candidate status");
    }
  };

  const handleViewDetails = async (candidate) => {
    try {
      // Fetch full candidate details
      const fullDetails = await candidateApi.getCandidateById(
        jobId,
        candidate.id
      );
      setSelectedCandidate(fullDetails);
      setShowModal(true);
    } catch (err) {
      console.error("Error fetching candidate details:", err);
      alert("Failed to load candidate details");
    }
  };

  const calculateStats = () => {
    const total = candidates.length;
    const pending = candidates.filter((c) => c.status === "Pending").length;
    const approved = candidates.filter((c) => c.status === "Approved").length;
    const rejected = candidates.filter((c) => c.status === "Rejected").length;
    const avgScore =
      candidates
        .filter((c) => c.aiAnalysis?.overallScore)
        .reduce((sum, c) => sum + c.aiAnalysis.overallScore, 0) / total || 0;

    return {
      total,
      pending,
      approved,
      rejected,
      avgScore: avgScore.toFixed(1),
    };
  };

  if (loading) {
    return (
      <div className="loading-screen">
        <Loader className="spinner-large" size={48} />
        <p>Loading dashboard...</p>
      </div>
    );
  }

  if (error || !job) {
    return (
      <div className="error-screen">
        <h2>Error Loading Dashboard</h2>
        <p>{error}</p>
        <button onClick={() => navigate("/jobs")} className="btn-primary">
          Back to Jobs
        </button>
      </div>
    );
  }

  const stats = calculateStats();

  return (
    <div className="hr-dashboard">
      {/* Header Section */}
      <div className="dashboard-header">
        <button onClick={() => navigate("/jobs")} className="back-button">
          <ArrowLeft size={20} />
          Back to Jobs
        </button>

        <div className="job-header-info">
          <div className="job-title-section">
            <h1>{job.title}</h1>
            <span className={`status-pill status-${job.status.toLowerCase()}`}>
              {job.status}
            </span>
          </div>

          <div className="job-meta-info">
            <div className="meta-item">
              <Building size={16} />
              <span>{job.companyName}</span>
            </div>
            <div className="meta-item">
              <MapPin size={16} />
              <span>{job.location}</span>
            </div>
            <div className="meta-item">
              <Briefcase size={16} />
              <span>{job.employmentType}</span>
            </div>
            <div className="meta-item">
              <Calendar size={16} />
              <span>
                Posted: {new Date(job.postedDate).toLocaleDateString()}
              </span>
            </div>
          </div>
        </div>
      </div>

      {/* Stats Cards */}
      <div className="stats-grid">
        <div className="stat-card">
          <div className="stat-icon total">
            <Users size={24} />
          </div>
          <div className="stat-content">
            <h3>{stats.total}</h3>
            <p>Total Candidates</p>
          </div>
        </div>

        <div className="stat-card">
          <div className="stat-icon pending">
            <Loader size={24} />
          </div>
          <div className="stat-content">
            <h3>{stats.pending}</h3>
            <p>Pending Review</p>
          </div>
        </div>

        <div className="stat-card">
          <div className="stat-icon approved">
            <Users size={24} />
          </div>
          <div className="stat-content">
            <h3>{stats.approved}</h3>
            <p>Approved</p>
          </div>
        </div>

        <div className="stat-card">
          <div className="stat-icon rejected">
            <Users size={24} />
          </div>
          <div className="stat-content">
            <h3>{stats.rejected}</h3>
            <p>Rejected</p>
          </div>
        </div>

        <div className="stat-card">
          <div className="stat-icon score">
            <TrendingUp size={24} />
          </div>
          <div className="stat-content">
            <h3>{stats.avgScore}</h3>
            <p>Average AI Score</p>
          </div>
        </div>

        {job.salaryMin > 0 && (
          <div className="stat-card">
            <div className="stat-icon salary">
              <DollarSign size={24} />
            </div>
            <div className="stat-content">
              <h3>${job.salaryMin.toLocaleString()}</h3>
              <p>Salary Range</p>
            </div>
          </div>
        )}
      </div>

      {/* Job Details Section */}
      <div className="job-details-section">
        <div className="details-card">
          <h2>Job Description</h2>
          <p>{job.description}</p>
        </div>

        <div className="details-card">
          <h2>Requirements</h2>
          <p>{job.requirements}</p>
        </div>
      </div>

      {/* Candidates List */}
      <CandidateList
        candidates={candidates}
        onStatusUpdate={handleStatusUpdate}
        onViewDetails={handleViewDetails}
      />

      {/* Candidate Details Modal */}
      {showModal && selectedCandidate && (
        <CandidateDetailsModal
          candidate={selectedCandidate}
          onClose={() => setShowModal(false)}
          onStatusUpdate={handleStatusUpdate}
        />
      )}
    </div>
  );
};

export default HrDashboard;
