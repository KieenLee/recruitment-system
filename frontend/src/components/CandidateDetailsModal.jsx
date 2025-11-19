import {
  X,
  User,
  Mail,
  Phone,
  Calendar,
  FileText,
  Award,
  AlertTriangle,
} from "lucide-react";
import "../css/CandidateDetailsModal.css";

const CandidateDetailsModal = ({ candidate, onClose, onStatusUpdate }) => {
  const handleStatusChange = (newStatus) => {
    onStatusUpdate(candidate.id, newStatus);
    onClose();
  };

  const getScoreColor = (score) => {
    if (score >= 8) return "#10b981";
    if (score >= 6) return "#f59e0b";
    return "#ef4444";
  };

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-content" onClick={(e) => e.stopPropagation()}>
        <div className="modal-header">
          <h2>Candidate Details</h2>
          <button className="close-button" onClick={onClose}>
            <X size={24} />
          </button>
        </div>

        <div className="modal-body">
          {/* Basic Info */}
          <section className="info-section">
            <h3>
              <User size={20} /> Personal Information
            </h3>
            <div className="info-grid">
              <div className="info-item">
                <span className="info-label">Full Name:</span>
                <span className="info-value">{candidate.fullName}</span>
              </div>
              <div className="info-item">
                <span className="info-label">
                  <Mail size={16} /> Email:
                </span>
                <span className="info-value">{candidate.email}</span>
              </div>
              <div className="info-item">
                <span className="info-label">
                  <Phone size={16} /> Phone:
                </span>
                <span className="info-value">{candidate.phone}</span>
              </div>
              <div className="info-item">
                <span className="info-label">
                  <Calendar size={16} /> Applied:
                </span>
                <span className="info-value">
                  {new Date(candidate.uploadedAt).toLocaleString()}
                </span>
              </div>
              <div className="info-item">
                <span className="info-label">
                  <FileText size={16} /> CV File:
                </span>
                <span className="info-value">{candidate.cvFileName}</span>
              </div>
              <div className="info-item">
                <span className="info-label">Status:</span>
                <span
                  className={`status-badge-large status-${candidate.status.toLowerCase()}`}
                >
                  {candidate.status}
                </span>
              </div>
            </div>
          </section>

          {/* AI Analysis */}
          {candidate.aiAnalysis && (
            <>
              {/* Overall Score */}
              <section className="info-section">
                <h3>
                  <Award size={20} /> AI Analysis Results
                </h3>
                <div className="score-container">
                  <div className="overall-score">
                    <div
                      className="score-circle"
                      style={{
                        borderColor: getScoreColor(
                          candidate.aiAnalysis.overallScore
                        ),
                      }}
                    >
                      <span className="score-number">
                        {candidate.aiAnalysis.overallScore}
                      </span>
                      <span className="score-max">/10</span>
                    </div>
                    <p className="score-label">Overall Match Score</p>
                  </div>
                  <div className="analysis-date">
                    <Calendar size={16} />
                    Analyzed:{" "}
                    {new Date(candidate.aiAnalysis.analyzedAt).toLocaleString()}
                  </div>
                </div>
              </section>

              {/* Summary */}
              <section className="info-section">
                <h3>AI Summary</h3>
                <div className="summary-box">
                  <p>{candidate.aiAnalysis.summary}</p>
                </div>
              </section>

              {/* Extracted Information */}
              {candidate.aiAnalysis.extractedInformation && (
                <section className="info-section">
                  <h3>Extracted from CV</h3>
                  <div className="extracted-info">
                    {candidate.aiAnalysis.extractedInformation.skills?.length >
                      0 && (
                      <div className="extracted-group">
                        <h4>Skills</h4>
                        <div className="tag-list">
                          {candidate.aiAnalysis.extractedInformation.skills.map(
                            (skill, index) => (
                              <span key={index} className="tag">
                                {skill}
                              </span>
                            )
                          )}
                        </div>
                      </div>
                    )}

                    {candidate.aiAnalysis.extractedInformation.education
                      ?.length > 0 && (
                      <div className="extracted-group">
                        <h4>Education</h4>
                        <ul className="simple-list">
                          {candidate.aiAnalysis.extractedInformation.education.map(
                            (edu, index) => (
                              <li key={index}>{edu}</li>
                            )
                          )}
                        </ul>
                      </div>
                    )}

                    {candidate.aiAnalysis.extractedInformation.experience
                      ?.length > 0 && (
                      <div className="extracted-group">
                        <h4>Experience</h4>
                        <ul className="simple-list">
                          {candidate.aiAnalysis.extractedInformation.experience.map(
                            (exp, index) => (
                              <li key={index}>{exp}</li>
                            )
                          )}
                        </ul>
                      </div>
                    )}
                  </div>
                </section>
              )}

              {/* Criteria Evaluation */}
              {candidate.aiAnalysis.criteriaEvaluation?.length > 0 && (
                <section className="info-section">
                  <h3>Criteria Evaluation</h3>
                  <div className="criteria-list">
                    {candidate.aiAnalysis.criteriaEvaluation.map(
                      (criterion, index) => (
                        <div key={index} className="criterion-card">
                          <div className="criterion-header">
                            <h4>{criterion.criterion}</h4>
                            <div className="criterion-score">
                              <span
                                className="score-badge"
                                style={{
                                  backgroundColor: getScoreColor(
                                    criterion.score
                                  ),
                                }}
                              >
                                {criterion.score}/10
                              </span>
                              <span
                                className={`met-badge met-${criterion.isMet.toLowerCase()}`}
                              >
                                {criterion.isMet}
                              </span>
                            </div>
                          </div>
                          <div className="criterion-evidence">
                            <strong>Evidence:</strong>
                            <p>"{criterion.evidence}"</p>
                          </div>
                        </div>
                      )
                    )}
                  </div>
                </section>
              )}

              {/* Red Flags */}
              {candidate.aiAnalysis.redFlags?.length > 0 && (
                <section className="info-section red-flags-section">
                  <h3>
                    <AlertTriangle size={20} /> Red Flags
                  </h3>
                  <ul className="red-flags-list">
                    {candidate.aiAnalysis.redFlags.map((flag, index) => (
                      <li key={index}>
                        <AlertTriangle size={16} />
                        {flag}
                      </li>
                    ))}
                  </ul>
                </section>
              )}
            </>
          )}

          {!candidate.aiAnalysis && (
            <section className="info-section">
              <div className="ai-pending">
                <Loader className="spinner" size={32} />
                <p>AI Analysis is being processed...</p>
                <small>
                  This usually takes 1-2 minutes. Please refresh the page later.
                </small>
              </div>
            </section>
          )}
        </div>

        {/* Modal Footer */}
        <div className="modal-footer">
          {candidate.status === "Pending" && (
            <>
              <button
                className="btn-modal btn-approve"
                onClick={() => handleStatusChange("Approved")}
              >
                Approve Candidate
              </button>
              <button
                className="btn-modal btn-reject"
                onClick={() => handleStatusChange("Rejected")}
              >
                Reject Candidate
              </button>
            </>
          )}
          {candidate.status !== "Pending" && (
            <button className="btn-modal btn-secondary" onClick={onClose}>
              Close
            </button>
          )}
        </div>
      </div>
    </div>
  );
};

export default CandidateDetailsModal;
