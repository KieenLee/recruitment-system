import React, { useEffect, useState } from "react";
import { getCandidatesByJobId } from "../api/candidateService";
import {
  User,
  Mail,
  Phone,
  Calendar,
  CheckCircle,
  XCircle,
  Clock,
} from "lucide-react";
import "../css/CandidateList.css";

const CandidateList = ({ jobId }) => {
  const [candidates, setCandidates] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchCandidates = async () => {
      try {
        const data = await getCandidatesByJobId(jobId);
        setCandidates(data);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    fetchCandidates();
  }, [jobId]);

  const [selectedStatus, setSelectedStatus] = useState("All");

  const getStatusIcon = (status) => {
    switch (status.toLowerCase()) {
      case "approved":
        return <CheckCircle size={20} className="status-icon approved" />;
      case "rejected":
        return <XCircle size={20} className="status-icon rejected" />;
      default:
        return <Clock size={20} className="status-icon pending" />;
    }
  };

  const getStatusClass = (status) => {
    return `status-badge status-${status.toLowerCase()}`;
  };

  const getScoreClass = (score) => {
    if (score >= 8) return "score-high";
    if (score >= 6) return "score-medium";
    return "score-low";
  };

  const filteredCandidates =
    selectedStatus === "All"
      ? candidates
      : candidates.filter((c) => c.status === selectedStatus);

  if (loading) {
    return <div>Loading candidates...</div>;
  }

  if (error) {
    return <div>Error fetching candidates: {error}</div>;
  }

  return (
    <div className="candidate-list-container">
      <div className="list-header">
        <h2>Candidates ({filteredCandidates.length})</h2>
        <div className="filter-tabs">
          {["All", "Pending", "Approved", "Rejected"].map((status) => (
            <button
              key={status}
              className={`filter-tab ${
                selectedStatus === status ? "active" : ""
              }`}
              onClick={() => setSelectedStatus(status)}
            >
              {status}
            </button>
          ))}
        </div>
      </div>

      <div className="candidates-grid">
        {filteredCandidates.map((candidate) => (
          <div key={candidate.id} className="candidate-card">
            <div className="candidate-header">
              <div className="candidate-info">
                <h3>{candidate.fullName}</h3>
                <div className={getStatusClass(candidate.status)}>
                  {getStatusIcon(candidate.status)}
                  {candidate.status}
                </div>
              </div>
              {candidate.aiAnalysis && (
                <div
                  className={`score-badge ${getScoreClass(
                    candidate.aiAnalysis.overallScore
                  )}`}
                >
                  <span className="score-value">
                    {candidate.aiAnalysis.overallScore}
                  </span>
                  <span className="score-label">/10</span>
                </div>
              )}
            </div>

            <div className="candidate-details">
              <div className="detail-item">
                <Mail size={16} />
                <span>{candidate.email}</span>
              </div>
              <div className="detail-item">
                <Phone size={16} />
                <span>{candidate.phone}</span>
              </div>
              <div className="detail-item">
                <Calendar size={16} />
                <span>
                  Applied: {new Date(candidate.uploadedAt).toLocaleDateString()}
                </span>
              </div>
            </div>

            {candidate.aiAnalysis && (
              <div className="ai-summary">
                <h4>AI Analysis Summary</h4>
                <p>{candidate.aiAnalysis.summary}</p>
              </div>
            )}

            <div className="candidate-actions">
              <button
                className="btn-view"
                onClick={() => onViewDetails(candidate)}
              >
                View Details
              </button>
              {candidate.status === "Pending" && (
                <>
                  <button
                    className="btn-approve"
                    onClick={() => onStatusUpdate(candidate.id, "Approved")}
                  >
                    <CheckCircle size={18} />
                    Approve
                  </button>
                  <button
                    className="btn-reject"
                    onClick={() => onStatusUpdate(candidate.id, "Rejected")}
                  >
                    <XCircle size={18} />
                    Reject
                  </button>
                </>
              )}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};

export default CandidateList;
