import "../styles/globals.scss";
import { useState } from "react";

function MyApp({ Component, pageProps }) {
	const [darkmode, setDarkMode] = useState(true);

	console.log(darkmode)

	const toggleDarkMode = () => {
		setDarkMode(!darkmode);
	};

	return <Component {...pageProps} darkmode={darkmode} toggleDarkMode={toggleDarkMode}/>;
}

export default MyApp;
