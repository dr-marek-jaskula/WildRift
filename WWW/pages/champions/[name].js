import PropTypes from "prop-types";
// eslint-disable-next-line no-undef
process.env.NODE_TLS_REJECT_UNAUTHORIZED="0";
import Head from "next/head";

export default function ChampionPage (props) {
	const { params, darkmode, toggleDarkMode } = props;
	const { championData } = params;
	const { name } = championData;
	console.log(props);
	return (
		<div style={darkmode? {background: 'black', color: 'white'} : {background: 'white', color: 'black'}}>
			<Head>
				<title>{name}</title>
			</Head>
			<div>Hello {name}</div>
			<button onClick={toggleDarkMode}>Dark mode</button>
		</div>
	);
}

ChampionPage.propTypes = {
	params: PropTypes.object,
};

export async function getStaticProps({params}) {
	const req = await fetch(`https://localhost:5001/api/Champion/${params.name}`, {
		method: "GET",
		mode: "cors",
		cache: "default",
		headers: {
			"Content-Type":"application/json",
		}});
	const data = await req.json();
	if (!data) {
		return {
			notFound: true,
		};
	}
	return {
		props: {
			params: {
				championData: data,
			}
		}
	};
}

export async function getStaticPaths() {
	const req = await fetch("https://localhost:5001/api/Champion/Names", {
		method: "GET",
		mode: "cors",
		cache: "default",
		headers: {
			"Content-Type":"application/json",
		}});
	const data = await req.json();
	const paths = data.map(name => {
		return { params: {
			name
		}};
	});
	return {
		paths,
		fallback: false
	};
}