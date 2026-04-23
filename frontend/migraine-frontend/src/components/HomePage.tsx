import { Link } from "react-router-dom";

type ForecastItem = {
  day: string;
  risk: string;
  color: string;
};

export const HomePage = () => {
  const forecastData: ForecastItem[] = [
    { day: "Mon", risk: "Neutral", color: "secondary" },
    { day: "Tue", risk: "Beneficial", color: "success" },
    { day: "Wed", risk: "At Risk", color: "warning" },
    { day: "Thu", risk: "At High Risk", color: "danger" },
    { day: "Fri", risk: "At Extreme Risk", color: "dark" },
  ];

  return (
    <div className="container py-5 mt-4">

      {/* --- HERO SECTION --- */}
<div className="row justify-content-center text-center mb-5">
  <div className="col-lg-8">
    <h1 className="display-4 fw-bold mb-3">
      Predict & Prevent <br />
      <span
        style={{
          background: "linear-gradient(45deg, #0d6efd, #6610f2)",
          WebkitBackgroundClip: "text",
          WebkitTextFillColor: "transparent",
        }}
      >
        Migraine Attacks
      </span>
    </h1>

    <p className="lead text-muted mb-4">
      Stay ahead of migraines with intelligent forecasts based on environmental
      conditions, pressure changes, and personalized triggers.
    </p>

    <div className="d-flex gap-3 justify-content-center">
      <Link
        to="/assignHealthCondition"
        className="btn btn-primary px-4 py-2 rounded-pill shadow-sm fw-semibold"
        style={{
          background: "linear-gradient(45deg, #0d6efd, #6610f2)",
          border: "none",
        }}
      >
        Check Forecast
      </Link>

      <Link
        to="/symptoms"
        className="btn btn-outline-dark px-4 py-2 rounded-pill"
      >
        Explore Symptoms
      </Link>
    </div>
  </div>
</div>

      {/* --- WEEKLY FORECAST --- */}
      <div className="mb-5">
        <h5 className="text-uppercase text-muted small fw-bold mb-4">
          Weekly Migraine Forecast
        </h5>

        <div className="row g-3">
          {forecastData.map((d, i) => (
            <div key={i} className="col-6 col-md-4 col-lg">
              <div className="card border-0 shadow-sm rounded-4 h-100 text-center p-3 hover-card">
                <p className="text-muted small mb-2">{d.day}</p>

                <div
                  className={`bg-${d.color} rounded-circle mx-auto mb-3`}
                  style={{ width: "14px", height: "14px" }}
                ></div>

                <p className="fw-semibold m-0">{d.risk}</p>
              </div>
            </div>
          ))}
        </div>
      </div>

      {/* --- FEATURE CARDS --- */}
      <div className="row g-4 mt-5">
        <div className="col-md-4">
          <div className="p-4 bg-white shadow-sm rounded-4 h-100 border-0">
            <h5 className="fw-bold mb-2">📊 Smart Forecasting</h5>
            <p className="text-muted small">
               Predictions based on weather, pressure patterns.
            </p>
          </div>
        </div>

        <div className="col-md-4">
          <div className="p-4 bg-white shadow-sm rounded-4 h-100 border-0">
            <h5 className="fw-bold mb-2">🧠 Trigger Insights</h5>
            <p className="text-muted small">
              Understand what causes your migraines and how to avoid them.
            </p>
          </div>
        </div>

        <div className="col-md-4">
          <div className="p-4 bg-white shadow-sm rounded-4 h-100 border-0">
            <h5 className="fw-bold mb-2">📅 Daily Tracking</h5>
            <p className="text-muted small">
              Log symptoms and monitor patterns to improve long-term health.
            </p>
          </div>
        </div>
      </div>

      {/* --- EXTRA STYLING --- */}
      <style>
        {`
          .hover-card {
            transition: all 0.25s ease;
          }

          .hover-card:hover {
            transform: translateY(-5px);
            box-shadow: 0 8px 20px rgba(0,0,0,0.08);
          }
        `}
      </style>
    </div>
  );
};

export default HomePage;