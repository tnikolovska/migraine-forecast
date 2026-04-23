import { Link, NavLink } from "react-router-dom";
import "bootstrap/dist/css/bootstrap.min.css";

const Navbar = () => {
  // Auth state from localStorage
  const token = localStorage.getItem("token");
  const role = localStorage.getItem("role");

  const isLoggedIn = !!token;
  //const isAdmin = role?.toLowerCase() === "admin";

  const handleLogout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("role");
    window.location.href = "/";
  };

  return (
    <nav className="navbar navbar-expand-lg bg-white shadow-sm sticky-top py-3">
      <div className="container">

        {/* LOGO */}
        <Link className="navbar-brand d-flex align-items-center fw-bold" to="/">
          <span className="fs-3 me-2">🧠</span>
          <span
            className="fs-4"
            style={{
              background: "linear-gradient(45deg, #0d6efd, #6610f2)",
              WebkitBackgroundClip: "text",
              WebkitTextFillColor: "transparent",
            }}
          >
            MigraineMagic
          </span>
        </Link>

        {/* HAMBURGER */}
        <button
          className="navbar-toggler border-0 shadow-none"
          type="button"
          data-bs-toggle="collapse"
          data-bs-target="#navbarNav"
        >
          <span className="navbar-toggler-icon"></span>
        </button>

        <div className="collapse navbar-collapse" id="navbarNav">

          {/* LEFT LINKS */}
          <ul className="navbar-nav me-auto mb-2 mb-lg-0 ms-lg-4 gap-lg-2">

            <li className="nav-item">
              <NavLink
                to="/"
                className={({ isActive }) =>
                  `nav-link px-3 ${isActive ? "fw-semibold text-primary" : "text-dark"}`
                }
              >
                Home
              </NavLink>
            </li>

            <li className="nav-item">
              <NavLink
                to="/migraineHeadacheDetails"
                className={({ isActive }) =>
                  `nav-link px-3 ${isActive ? "fw-semibold text-primary" : "text-dark"}`
                }
              >
                Details
              </NavLink>
            </li>

            <li className="nav-item">
              <NavLink
                to="/symptoms"
                className={({ isActive }) =>
                  `nav-link px-3 ${isActive ? "fw-semibold text-primary" : "text-dark"}`
                }
              >
                Symptoms
              </NavLink>
            </li>
          </ul>

          {/* RIGHT SIDE */}
          <div className="d-flex align-items-center gap-3">

            {/* ADMIN LINKS (ONLY FOR ADMIN) */}
            {role === "Admin" && (
              <>
                <Link
                  to="/admin/create-condition"
                  className="btn btn-outline-primary btn-sm rounded-pill px-3"
                >
                  ➕ Add Condition
                </Link>

                <Link
                  to="/admin/create-symptom"
                  className="btn btn-outline-primary btn-sm rounded-pill px-3"
                >
                  ➕ Add Symptom
                </Link>
              </>
            )}

            {/* AUTH SECTION */}
            {!isLoggedIn ? (
              <>
                <Link
                  to="/login"
                  className="btn btn-link text-dark fw-medium text-decoration-none"
                >
                  Login
                </Link>

                <Link
                  to="/register"
                  className="btn btn-primary rounded-pill px-4 shadow-sm fw-semibold"
                  style={{
                    background: "linear-gradient(45deg, #0d6efd, #6610f2)",
                    border: "none",
                  }}
                >
                  Join Free
                </Link>
              </>
            ) : (
              <button
                onClick={handleLogout}
                className="btn btn-outline-danger btn-sm rounded-pill px-3"
              >
                Logout
              </button>
            )}

          </div>
        </div>
      </div>
    </nav>
  );
};

export default Navbar;