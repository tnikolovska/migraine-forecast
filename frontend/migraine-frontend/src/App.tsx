import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Navbar from './components/Navbar';
import HomePage from './components/HomePage'; 
import ForecastList from './ForecastList'; 
import CreateSymptom from './admin/CreateSymptom'; 
import CreateHealthCondition from './admin/CreateHealthCondition';
import HeadacheDetails from './HeadacheDetails';

// Bootstrap & Custom Styles
import 'bootstrap/dist/css/bootstrap.min.css';
import './App.css';
import UpdateHealthCondition from './admin/UpdateHealthCondition';

import Login from "./components/Login";
import Register from "./components/Register";
import EditSymptom from './admin/EditSymptom';
import AssignHealthCondition from './AssignHealthCondition';
import UserSymptomSelection from './UserSymptomSelection';
import Forecast from './Forecast';
import ProtectedRoute from "./ProtectedRoute";

function App() {
  return (
    <Router>
      {/* The 'bg-light' and 'min-vh-100' classes ensure the 
        background color is consistent even on short pages 
      */}
      <div className="App bg-light min-vh-100">
        <Navbar />

        {/* Main content area with padding 
          This ensures your content isn't squashed against the navbar
        */}
        <main className="py-4">
          <Routes>
            <Route path="/" element={<HomePage />} />
            
            <Route path="/migraineHeadacheDetails" element={<HeadacheDetails />} />
            
            <Route path="/symptoms" element={<ForecastList />} />

           <Route
              path="/assignHealthCondition"
              element={
                <ProtectedRoute>
                  <AssignHealthCondition />
                </ProtectedRoute>
              }
            />

             <Route path="/userSymptomSelection" element={ <ProtectedRoute><UserSymptomSelection /></ProtectedRoute>} />
             <Route path="/forecast" element={<ProtectedRoute><Forecast /></ProtectedRoute>} />

            {/* Admin Routes */}
            <Route path="/admin/create-condition" element={<CreateHealthCondition />} />           
            <Route path="/admin/create-symptom" element={<CreateSymptom />} />

            <Route path="/admin/update-condition/:id" element={<UpdateHealthCondition />} />

            <Route path="/admin/update-symptom/:id" element={<EditSymptom />} />

            {/* Auth Routes with a fancy placeholder */}
            <Route path="/login" element={<Login />} />
            
            <Route path="/register" element={<Register/>}/>
              
          </Routes>
        </main>

        {/* Simple Fabulous Footer */}
        <footer className="py-4 text-center text-muted border-top bg-white mt-auto">
          <small>&copy; 2026 MigraineMagic ✨ Personalized Forecast Dashboard</small>
        </footer>
      </div>
    </Router>
  );
}

export default App;