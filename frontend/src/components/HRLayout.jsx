import React from "react";
import { Outlet, useNavigate, useLocation } from "react-router-dom";
import {
  LayoutDashboard,
  Briefcase,
  Users,
  LogOut,
  Menu,
  X,
} from "lucide-react";
import "../css/HRLayout.css";

const HRLayout = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const [sidebarOpen, setSidebarOpen] = React.useState(true);

  const userName = localStorage.getItem("userName") || "HR Manager";
  const userRole = localStorage.getItem("userRole") || "HR";

  const handleLogout = () => {
    localStorage.removeItem("isAuthenticated");
    localStorage.removeItem("userRole");
    localStorage.removeItem("userName");
    navigate("/login");
  };

  const menuItems = [
    { path: "/hr/dashboard", icon: LayoutDashboard, label: "Dashboard" },
    { path: "/hr/jobs", icon: Briefcase, label: "Job Postings" },
    { path: "/hr/candidates", icon: Users, label: "All Candidates" },
  ];

  return (
    <div className="hr-layout">
      {/* Sidebar */}
      <aside className={`sidebar ${sidebarOpen ? "open" : "closed"}`}>
        <div className="sidebar-header">
          <h2>🎯 TalentHub</h2>
          <button
            className="sidebar-toggle"
            onClick={() => setSidebarOpen(!sidebarOpen)}
          >
            {sidebarOpen ? <X size={20} /> : <Menu size={20} />}
          </button>
        </div>

        <nav className="sidebar-nav">
          {menuItems.map((item) => (
            <button
              key={item.path}
              className={`nav-item ${
                location.pathname === item.path ? "active" : ""
              }`}
              onClick={() => navigate(item.path)}
            >
              <item.icon size={20} />
              {sidebarOpen && <span>{item.label}</span>}
            </button>
          ))}
        </nav>

        <div className="sidebar-footer">
          <div className="user-info">
            <div className="user-avatar">{userName.charAt(0)}</div>
            {sidebarOpen && (
              <div className="user-details">
                <p className="user-name">{userName}</p>
                <p className="user-role">{userRole}</p>
              </div>
            )}
          </div>
          <button className="btn-logout" onClick={handleLogout}>
            <LogOut size={20} />
            {sidebarOpen && <span>Logout</span>}
          </button>
        </div>
      </aside>

      {/* Main Content */}
      <main className="main-content">
        <Outlet />
      </main>
    </div>
  );
};

export default HRLayout;
