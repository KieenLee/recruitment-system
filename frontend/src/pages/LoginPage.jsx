import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Lock, Mail, ArrowLeft } from "lucide-react";
import "../css/LoginPage.css";

const LoginPage = () => {
  const navigate = useNavigate();
  const [formData, setFormData] = useState({
    email: "",
    password: "",
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
    setError("");
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    // TODO: Implement real authentication API call
    // For now, demo credentials
    if (
      formData.email === "hr@techcorp.com" &&
      formData.password === "password"
    ) {
      localStorage.setItem("isAuthenticated", "true");
      localStorage.setItem("userRole", "HR");
      localStorage.setItem("userName", "HR Manager");
      navigate("/hr/dashboard");
    } else if (
      formData.email === "admin@recruitment.com" &&
      formData.password === "admin"
    ) {
      localStorage.setItem("isAuthenticated", "true");
      localStorage.setItem("userRole", "Admin");
      localStorage.setItem("userName", "Administrator");
      navigate("/hr/dashboard");
    } else {
      setError("Invalid email or password");
    }
  };

  return (
    <div className="login-page">
      <button className="btn-back-home" onClick={() => navigate("/")}>
        <ArrowLeft size={20} />
        Back to Home
      </button>

      <div className="login-container">
        <div className="login-card">
          <div className="login-header">
            <h1>🎯 TalentHub</h1>
            <h2>HR Portal Login</h2>
            <p>Sign in to access the recruitment dashboard</p>
          </div>

          {error && <div className="error-alert">{error}</div>}

          <form onSubmit={handleSubmit} className="login-form">
            <div className="form-group">
              <label>Email Address</label>
              <div className="input-with-icon">
                <Mail size={20} className="input-icon" />
                <input
                  type="email"
                  name="email"
                  value={formData.email}
                  onChange={handleInputChange}
                  placeholder="hr@company.com"
                  required
                />
              </div>
            </div>

            <div className="form-group">
              <label>Password</label>
              <div className="input-with-icon">
                <Lock size={20} className="input-icon" />
                <input
                  type="password"
                  name="password"
                  value={formData.password}
                  onChange={handleInputChange}
                  placeholder="••••••••"
                  required
                />
              </div>
            </div>

            <div className="form-options">
              <label className="remember-me">
                <input type="checkbox" />
                <span>Remember me</span>
              </label>
              <a href="#" className="forgot-password">
                Forgot password?
              </a>
            </div>

            <button
              type="submit"
              className="btn-login-submit"
              disabled={loading}
            >
              {loading ? "Signing in..." : "Sign In"}
            </button>
          </form>

          <div className="demo-credentials">
            <p>
              <strong>Demo Credentials:</strong>
            </p>
            <p>Email: hr@techcorp.com</p>
            <p>Password: password</p>
          </div>
        </div>
      </div>
    </div>
  );
};

export default LoginPage;
