import React, { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
//import axios from "axios";
import axios, { AxiosError } from "axios";

const Register = () => {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);
  
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");

    // Osnovna validacija na frontendu
    if (password !== confirmPassword) {
      setError("Passwords do not match!");
      return;
    }

    setLoading(true);

    try {
      // PROVERI URL: Mora da se poklapa sa tvojim backend portom u Dockeru
      //const response = await axios.post("http://localhost:5000/api/auth/register", {
      const response = await axios.post("/api/auth/register", {  
       username: username,
        password: password,
      });

      if (response.status === 200 || response.status === 201) {
        alert("Registration successful! Please login.");
        navigate("/login");
      }
    } catch (err) {
        const axiosError = err as AxiosError<{ message?: string }>;
        setError(
            axiosError.response?.data?.message || 
            "Registration failed. Username might be taken."
        );
        } finally {
        setLoading(false);
        }
  };

  return (
    <div className="container d-flex justify-content-center align-items-center" style={{ minHeight: "80vh" }}>
      <div className="card shadow-lg border-0 rounded-4" style={{ maxWidth: "400px", width: "100%" }}>
        <div className="card-body p-5">
          <div className="text-center mb-4">
            <span className="fs-1">🧠</span>
            <h2 className="fw-bold mt-2">Join Magic</h2>
            <p className="text-muted">Create your account to start</p>
          </div>

          {error && (
            <div className="alert alert-danger py-2 small" role="alert">
              {error}
            </div>
          )}

          <form onSubmit={handleSubmit}>
            <div className="mb-3">
              <label className="form-label small fw-semibold">Username</label>
              <input
                type="text"
                className="form-control form-control-lg bg-light border-0 fs-6"
                placeholder="Choose a username"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
                required
              />
            </div>

            <div className="mb-3">
              <label className="form-label small fw-semibold">Password</label>
              <input
                type="password"
                className="form-control form-control-lg bg-light border-0 fs-6"
                placeholder="••••••••"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                required
              />
            </div>

            <div className="mb-4">
              <label className="form-label small fw-semibold">Confirm Password</label>
              <input
                type="password"
                className="form-control form-control-lg bg-light border-0 fs-6"
                placeholder="••••••••"
                value={confirmPassword}
                onChange={(e) => setConfirmPassword(e.target.value)}
                required
              />
            </div>

            <button
              type="submit"
              className="btn btn-primary w-100 py-2 fw-bold rounded-pill shadow-sm"
              style={{
                background: "linear-gradient(45deg, #0d6efd, #6610f2)",
                border: "none",
              }}
              disabled={loading}
            >
              {loading ? "Creating account..." : "Register Now"}
            </button>
          </form>

          <div className="text-center mt-4">
            <p className="small text-muted">
              Already have an account?{" "}
              <Link to="/login" className="text-primary fw-bold text-decoration-none">
                Login
              </Link>
            </p>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Register;