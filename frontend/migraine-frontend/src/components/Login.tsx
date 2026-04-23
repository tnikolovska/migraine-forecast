import { useState } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";


const Login = () => {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const navigate = useNavigate();

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");
    setLoading(true);

    try {
      const response = await axios.post(
        "http://localhost:5000/api/auth/login", // CHANGE to your API URL
        {
          username,
          password,
        }
      );

      // EXPECTED RESPONSE:
      // { token: "...", role: "Admin" }

      localStorage.setItem("token", response.data.token);
      localStorage.setItem("role", response.data.role);
      window.location.href = "/";

      //setLoading(false);

      // redirect after login
      navigate("/");
    } catch (err) {
  setLoading(false);

  if (axios.isAxiosError(err)) {
    setError(
      err.response?.data?.message || "Invalid username or password"
    );
  } else {
    setError("Something went wrong");
  }
}
  };

  return (
    <div className="container d-flex justify-content-center align-items-center min-vh-100">
      <div className="card shadow-sm border-0 p-4 rounded-4" style={{ width: "400px" }}>
        
        <h3 className="text-center text-primary fw-bold mb-3">
          Login
        </h3>

        {error && (
          <div className="alert alert-danger py-2 text-center">
            {error}
          </div>
        )}

        <form onSubmit={handleLogin}>

          {/* Username */}
          <div className="mb-3">
            <label className="form-label">Username</label>
            <input
              type="text"
              className="form-control"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              placeholder="Enter username"
              required
            />
          </div>

          {/* Password */}
          <div className="mb-3">
            <label className="form-label">Password</label>
            <input
              type="password"
              className="form-control"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              placeholder="Enter password"
              required
            />
          </div>

          {/* Submit */}
          <button
            type="submit"
            className="btn btn-primary w-100 rounded-pill"
            disabled={loading}
          >
            {loading ? "Logging in..." : "Login"}
          </button>
        </form>

        <p className="text-center text-muted mt-3 mb-0">
          Don't have an account? <a href="/register">Register</a>
        </p>

      </div>
    </div>
  );
};

export default Login;