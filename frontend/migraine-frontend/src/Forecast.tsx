import React, { useEffect, useState } from "react";
import axios from "axios";

import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  BarElement,
  Tooltip,
  Legend,
} from "chart.js";
import ChartDataLabels from "chartjs-plugin-datalabels";
import { Bar } from "react-chartjs-2";
import type { ChartOptions, TooltipItem } from "chart.js";

// Register Chart.js components
ChartJS.register(CategoryScale, LinearScale, BarElement, Tooltip, Legend, ChartDataLabels);

// Types
interface ForecastDto {
  idForecast: string;
  name: string;
  date: string;
  value: number;
  category: string;
  categoryValue: number;
  mobileLink: string;
  link: string;
}

const Forecast: React.FC = () => {
  const [data, setData] = useState<ForecastDto[]>([]);

  useEffect(() => {
  const token = localStorage.getItem("token");

  axios
    //.get("http://localhost:5000/api/forecast", {
    .get("/api/forecast", {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    })
    .then((res) => {
      console.log("API RESPONSE:", res.data);
      setData(res.data.data);
    })
    .catch((err) => console.error(err));
}, []);

  // 🔥 CATEGORY LOGIC (CORRECT)
  const getCategory = (value: number) => {
    if (value <= 1.99) return { label: "Beneficial", color: "#0d6efd", icon: "🙂" };
    if (value <= 3.99) return { label: "Neutral", color: "#6c757d", icon: "😐" };
    if (value <= 5.99) return { label: "At Risk", color: "#ffc107", icon: "⚠️" };
    if (value <= 7.99) return { label: "At High Risk", color: "#fd7e14", icon: "🔥" };
    return { label: "At Extreme Risk", color: "#dc3545", icon: "💀" };
  };

  const chartData = {
    labels: data.map((f) =>
      new Date(f.date).toLocaleDateString("en-GB", {
        weekday: "short",
        day: "2-digit",
        month: "2-digit",
      })
    ),
    datasets: [
      {
        label: "Migraine Risk",
        data: data.map((f) => f.value),
        backgroundColor: data.map((f) => getCategory(f.value).color),
        borderRadius: 10,
      },
    ],
  };

  const options: ChartOptions<"bar"> = {
    responsive: true,
    plugins: {
      legend: { display: false },

      tooltip: {
        callbacks: {
          label: function (context: TooltipItem<"bar">) {
            const value = context.raw as number;
            const cat = getCategory(value);
            return `${cat.icon} ${cat.label} (${value})`;
          },
        },
      },

      datalabels: {
        color: "#000",
        font: { size: 22 },
        formatter: (value: number) => {
          return getCategory(value).icon;
        },
      },
    },

    scales: {
      y: {
        min: 0,
        max: 10,
        ticks: { stepSize: 1 },
        title: {
          display: true,
          text: "Migraine Risk Level",
        },
      },
      x: {
        title: {
          display: true,
          text: "Date",
        },
      },
    },
  };

  return (
    <div className="container mt-5">
      <div className="card shadow rounded-4 p-4">
        <h2 className="text-center mb-4">🧠 5-Day Migraine Forecast</h2>

        {data.length === 0 ? (
          <p className="text-center text-muted">Loading forecast...</p>
        ) : (
          <Bar data={chartData} options={options} />
        )}
      </div>
    </div>
  );
};

export default Forecast;