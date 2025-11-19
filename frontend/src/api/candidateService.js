import axios from "axios";

const API_BASE_URL =
  import.meta.env.VITE_API_BASE_URL || 'http://localhost:5175/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    "Content-Type": "application/json",
  },
});

// Job Postings API
export const jobPostingApi = {
  // Get all job postings
  getAllJobs: async (status = null) => {
    const params = status ? { status } : {};
    const response = await api.get("/jobpostings", { params });
    return response.data;
  },

  // Get job posting by ID
  getJobById: async (jobId) => {
    const response = await api.get(`/jobpostings/${jobId}`);
    return response.data;
  },

  // Create new job posting
  createJob: async (jobData) => {
    const response = await api.post("/jobpostings", jobData);
    return response.data;
  },

  // Update job posting
  updateJob: async (jobId, jobData) => {
    const response = await api.put(`/jobpostings/${jobId}`, jobData);
    return response.data;
  },

  // Delete job posting
  deleteJob: async (jobId) => {
    await api.delete(`/jobpostings/${jobId}`);
  },
};

// Candidates API
export const candidateApi = {
  // Get all candidates for a job
  getCandidatesByJobId: async (jobId, status = null) => {
    const params = status ? { status } : {};
    const response = await api.get(`/jobs/${jobId}/candidates`, { params });
    return response.data;
  },

  // Get candidate details
  getCandidateById: async (jobId, candidateId) => {
    const response = await api.get(`/jobs/${jobId}/candidates/${candidateId}`);
    return response.data;
  },

  // Apply to job (upload CV)
  applyToJob: async (jobId, candidateData, cvFile) => {
    const formData = new FormData();
    formData.append("fullName", candidateData.fullName);
    formData.append("email", candidateData.email);
    formData.append("phone", candidateData.phone);
    formData.append("cvFile", cvFile);

    const response = await api.post(
      `/jobs/${jobId}/candidates/apply`,
      formData,
      {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      }
    );
    return response.data;
  },

  // Update candidate status
  updateCandidateStatus: async (jobId, candidateId, status) => {
    const response = await api.patch(
      `/jobs/${jobId}/candidates/${candidateId}/status`,
      {
        status,
      }
    );
    return response.data;
  },
};

export default api;
